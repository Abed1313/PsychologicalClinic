using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Models.DTO;
using PsychologicalClinic.Repository.Interface;
using System.Security.Claims;

namespace PsychologicalClinic.Repository.Services
{
    public class AccountUserService :IAcountUser
    {
        private readonly UserManager<Characters> _userManager;
        private readonly SignInManager<Characters> _signInManager;
        private readonly JwtTokenServeses _jwtTokenServices;
        private readonly ClinicDbContext _context;
        private readonly MailjetEmailService _emailService;
        private readonly string _baseUrl;

        // Constructor
        public AccountUserService(UserManager<Characters> userManager,
                                  SignInManager<Characters> signInManager,
                                  JwtTokenServeses jwtTokenServices,
                                  ClinicDbContext context,
                                  MailjetEmailService emailService,
                                 IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtTokenServices = jwtTokenServices ?? throw new ArgumentNullException(nameof(jwtTokenServices));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService;
            _baseUrl = configuration["BaseUrl"];
        }


        public async Task<LogDTO> LoginUser(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
            {
                return null; // or return a custom error indicating invalid credentials
            }

            return new LogDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = await _jwtTokenServices.GenerateToken(user, TimeSpan.FromDays(14)),
                Roles = await _userManager.GetRolesAsync(user)
            };

        }
        // Logout
        public async Task<LogDTO> LogoutUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }

            await _signInManager.SignOutAsync();
            return new LogDTO
            {
                Id = user.Id,
                UserName = user.UserName
            };
        }

        // User Profile
        public async Task<LogDTO> UserProfile(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            return new LogDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = await _jwtTokenServices.GenerateToken(user, TimeSpan.FromMinutes(7)),
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        // Register
        public async Task<LogDTO> Register(RegisterUserDTO registerUserDTO, ModelStateDictionary modelState)
        {
            if (!registerUserDTO.Roles.Contains("Patient") && !registerUserDTO.Roles.Contains("Secretary") && !registerUserDTO.Roles.Contains("Doctor"))
            {
                throw new ArgumentException("User must have either the 'Patient' or 'Secretary' role to register.");
            }

            var account = new Characters
            {
                UserName = registerUserDTO.UserName,
                Email = registerUserDTO.Email,
            };

            var result = await _userManager.CreateAsync(account, registerUserDTO.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(account, registerUserDTO.Roles);

                foreach (var role in registerUserDTO.Roles)
                {
                    switch (role)
                    {
                        case "Admin":
                            // Check if there are already 3 admins
                            var adminCount = await _context.Doctors.CountAsync();
                            if (adminCount >= 3)
                            {
                                throw new InvalidOperationException("Cannot add more than 3 admins.");
                            }

                            var admin = new Doctor
                            {
                                CharactersId = account.Id,
                                Name = account.UserName,
                                Email = account.Email
                            };
                            _context.Doctors.Add(admin);
                            break;
                        case "Guest":
                            var guest = new Patient
                            {
                                CharactersId = account.Id,
                                Name = account.UserName
                            };
                            _context.Patients.Add(guest);
                            break;
                        case "Provider":
                            var provider = new Secretary
                            {
                                CharactersId = account.Id,
                                Name = account.UserName,
                                Email = account.Email
                            };
                            _context.Secretaries.Add(provider);
                            break;
                    }
                }
                await _context.SaveChangesAsync();
                return new LogDTO
                {
                    Id = account.Id,
                    UserName = account.UserName,
                    Token = await _jwtTokenServices.GenerateToken(account, TimeSpan.FromMinutes(7)),
                    Roles = await _userManager.GetRolesAsync(account)
                };
            }

            throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Delete User
        public async Task<LogDTO> DeleteAccount(string username)
        {
            var account = await _userManager.FindByNameAsync(username);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            await _userManager.DeleteAsync(account);
            return new LogDTO
            {
                Id = account.Id,
                UserName = account.UserName
            };
        }

        
        /////////////////////////////////////////////////////////////////////////////////////////////////
        
        private static int? ran;
        private static string? email;
        public async Task SendPasswordResetEmailAsync(string email1)
        {
            var resetEmail = await _userManager.FindByEmailAsync(email1);
            if (resetEmail != null)
            {
                ran = new Random().Next(1111, 9999);
                await _emailService.SendEmailAsync(resetEmail.Email, resetEmail.UserName, ran.ToString());
                email = email1;
            }

        }
        public bool ValidateCode(int code)
        {
            return ran.HasValue && code == ran; // Validate the code
        }
        public async Task<string> NewPassword(string newPassword, int c)

        {
            bool code = ValidateCode(c);
            if (code)
            {
                // Find the user by the stored email
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    // Set the new password directly
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                    // Update the user in the database
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return "Password has been reset successfully.";
                    }
                    return "Failed to reset password.";
                }
            }
            return "User not found.";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO model)
        {                                                        // To ChangePassword you must be Loged in 
                                                                 // Get the user by Id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            // Ensure the new password matches the confirmation
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "New password and confirmation do not match."
                });
            }

            // Change the user's password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                // Sign in the user again to refresh the security token
                await _signInManager.RefreshSignInAsync(user);
            }

            return result;
        }

    }
}
