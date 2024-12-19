using PsychologicalClinic.Models;
using PsychologicalClinic.Models.DTO;

namespace PsychologicalClinic.Repository.Interface
{
    public interface IQuiz
    {
        public Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        public Task<Quiz> GetQuizByIdAsync(int quizId);
        public Task<IEnumerable<Quiz>> GetQuizzesByDoctorIdAsync(string doctorId);
        public Task<Quiz> CreateQuizAsync(QuizCreationDTO quiz);
        public Task<bool> DeleteQuizAsync(int quizId);
        public Task<bool> AddQuestionsToQuizAsync(int quizId, List<QuestionDTO> questions);
        public Task<IEnumerable<QuizResult>> GetQuizResultsByPatientIdAsync(int patientId);
        public Task<QuizResult> SubmitQuizResultAsync(QuizResult quizResult);
        public Task<QuizResult> SubmitQuizAsync(QuizSubmissionDTO submission);
    }
}