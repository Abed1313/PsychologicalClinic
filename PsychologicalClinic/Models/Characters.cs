using Microsoft.AspNetCore.Identity;

namespace PsychologicalClinic.Models
{
    public class Characters : IdentityUser
    {
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }

    }
}
