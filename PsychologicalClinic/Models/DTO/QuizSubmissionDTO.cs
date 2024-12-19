namespace PsychologicalClinic.Models.DTO
{
    public class QuizSubmissionDTO
    {
        public int QuizId { get; set; }
        public int PatientId { get; set; }
        public List<AnswerDTO> Answers { get; set; }
    }
}