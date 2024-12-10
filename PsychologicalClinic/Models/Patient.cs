namespace PsychologicalClinic.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } // Can be an enum instead
        public string Nationality { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime FirstVisitDate { get; set; }

        // Navigation Properties
        public string CharactersId { get; set; }
        public Characters User { get; set; } // Navigation
        public ICollection<Disease> DiseaseHistory { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<PatientComment> Comments { get; set; }
    }
}
