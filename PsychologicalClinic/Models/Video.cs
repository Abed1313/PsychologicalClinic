namespace PsychologicalClinic.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; } // Can be an enum (Psychological, Cultural)

        // Foreign Key
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        // Additional Properties
        public int Likes { get; set; }
    }
}
