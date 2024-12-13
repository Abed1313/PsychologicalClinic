using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsychologicalClinic.Models.DTO;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using PsychologicalClinic.Repository.Services;

namespace PsychologicalClinic.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAcountUser _userManager;
        private readonly UserManager<Characters> _identityUserManager;
        private readonly MailjetEmailService _emailService;

        public AccountController(IAcountUser userManager, UserManager<Characters> identityUserManager, MailjetEmailService emailService)
        {
            _userManager = userManager;
            _identityUserManager = identityUserManager;
            _emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserDTO registerEmployeeDTO)
        {
            try
            {
                var validRoles = new List<string> { "Doctor", "Patient", "Secretary" };
                foreach (var role in registerEmployeeDTO.Roles)
                {
                    if (!validRoles.Contains(role))
                    {
                        return BadRequest($"Invalid role: {role}. Allowed roles are: Doctor, Patient, Secretary.");
                    }
                }

                var employee = await _userManager.Register(registerEmployeeDTO, this.ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Create and send the email after successful registration
                string emailBody = "<h1>Welcome to Clinic System</h1><p>Thank you for registering! You can now log in and start using the platform.</p>";
                bool emailSent = false;
                try
                {
                    emailSent = await _emailService.SendEmailAsync(registerEmployeeDTO.Email, "Welcome to Smart Home System", emailBody);
                }
                catch (Exception emailEx)
                {
                    // Log email sending exception if a logging service is available
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Registration succeeded, but email sending failed.", details = emailEx.Message });
                }

                if (!emailSent)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Registration succeeded, but email sending failed." });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logging service
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during registration.", details = ex.Message });
            }
        }


        [HttpPost("Login")]
        public async Task<ActionResult<LogDTO>> Login(LoginDTO loginDto)
        {
            var user = await _userManager.LoginUser(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }
            return user;
        }

        [HttpPost("Logout")]
        public async Task<LogDTO> Logout(string Username)
        {
            var user = await _userManager.LogoutUser(Username);
            return user;
        }


        [HttpGet("Profile")]
        public async Task<ActionResult<LogDTO>> Profile()
        {
            return await _userManager.UserProfile(User);
        }


        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            try
            {
                var account = await _userManager.DeleteAccount(username);
                if (account == null)
                {
                    return NotFound("Account not found");
                }
                return Ok(new { message = "Account deleted successfully.", account });
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logging service
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the account.", details = ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                await _userManager.SendPasswordResetEmailAsync(email);
                return Ok("Verification code sent to email.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ValidateCode")]
        public async Task<IActionResult> ValidateCode(int code)
        {
            try
            {
                var isValid = _userManager.ValidateCode(code);
                if (!isValid)
                {
                    return BadRequest("Invalid or expired code.");
                }
                return Ok("Code is valid.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(string newPassword, int code)
        {
            try
            {
                var result = await _userManager.NewPassword(newPassword, code);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _identityUserManager.GetUserId(User); // Get the currently logged-in user's ID
            var result = await _userManager.ChangePasswordAsync(userId, changePasswordDTO);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok("Password changed successfully.");
        }
    }
}
