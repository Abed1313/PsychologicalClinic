namespace PsychologicalClinic.Models
{
    public class PatientComment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public int Rating { get; set; } // 1 to 5

        // Foreign Keys
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
