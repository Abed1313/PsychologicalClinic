namespace PsychologicalClinic.Models
{
    public class Secretary
    {
        public int SecretaryId { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }

        // Navigation Properties
        public string CharactersId { get; set; }
        public Characters User { get; set; } // Navigation
        public ICollection<Appointment> ManagedAppointments { get; set; }
    }
}
