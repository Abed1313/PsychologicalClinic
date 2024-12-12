using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PsychologicalClinic.Models.DTO;
using System.Security.Claims;

namespace PsychologicalClinic.Repository.Interface
{
    public interface IAcountUser
    {
        // Add register
        Task<LogDTO> Register(RegisterUserDTO registerUserDTO, ModelStateDictionary modelState);

        // Add login
        Task<LogDTO> LoginUser(string username, string password);

        // Add logout
        Task<LogDTO> LogoutUser(string username);

        // Add delete account
        Task<LogDTO> DeleteAccount(string username);

        public Task<LogDTO> UserProfile(ClaimsPrincipal claimsPrincipal);

        Task SendPasswordResetEmailAsync(string email);
        bool ValidateCode(int code);
        Task<string> NewPassword(string newPassword, int c);
        public Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO model);
    }
}
