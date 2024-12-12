using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models.DTO
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
