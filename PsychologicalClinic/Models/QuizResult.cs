using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models
{
    public class QuizResult
    {
        public int QuizResultId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int TotalScore { get; set; }
        [MaxLength(500)]
        public string Feedback { get; set; } // Feedback based on the score
        public DateTime DateTaken { get; set; } = DateTime.Now;
    }
}