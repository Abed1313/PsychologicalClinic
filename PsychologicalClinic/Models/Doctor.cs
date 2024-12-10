using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PsychologicalClinic.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string PhotoUrl { get; set; }

        // Navigation Properties
        public string CharactersId { get; set; }
        public Characters User { get; set; } // Navigation
        public ICollection<Video> Videos { get; set; }
        public ICollection<PatientComment> Comments { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
