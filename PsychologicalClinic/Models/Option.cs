using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models
{
    public class Option
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        [MaxLength(500)]
        public string Text { get; set; }
        public int Score { get; set; } // Score for this option (e.g., 1 for low stress, 5 for high stress)
        public bool IsCorrect { get; set; }
    }
}
