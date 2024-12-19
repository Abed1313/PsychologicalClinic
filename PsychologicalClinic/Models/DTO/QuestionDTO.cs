namespace PsychologicalClinic.Models.DTO
{
    public class QuestionDTO
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int Score { get; set; }

        public int CorrectOptionIndex { get; set; }
    }
}