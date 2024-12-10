namespace PsychologicalClinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; } // Can be an enum (Pending, Accepted, Rejected)

        // Foreign Keys
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int? SecretaryId { get; set; }  // Nullable
        public Secretary Secretary { get; set; }
    }
}
