using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models.DTO
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "UserName Is Required. ")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email Is Required. ")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required. ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IList<string> Roles { get; set; }
    }
}
