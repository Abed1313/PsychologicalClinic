using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string Title { get; set; } 
        [MaxLength(1000)]
        public string Description { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public ICollection<QuizResult> QuizResults { get; set; } = new HashSet<QuizResult>();
    }
}