namespace PsychologicalClinic.Models
{
    public class Disease
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation Properties
        public ICollection<Patient> Patients { get; set; }
    }
}
