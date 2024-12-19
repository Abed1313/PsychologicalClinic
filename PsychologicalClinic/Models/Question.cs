using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations;

namespace PsychologicalClinic.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; }
        public int Weight { get; set; }
        public ICollection<Option> Options { get; set; } = new HashSet<Option>();
    }
}