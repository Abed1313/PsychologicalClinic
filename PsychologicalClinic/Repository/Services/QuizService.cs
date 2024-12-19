using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Models.DTO;

namespace PsychologicalClinic.Repository.Services
{
    public class QuizService : IQuiz
    {
        private readonly ClinicDbContext _context;

        public QuizService(ClinicDbContext context)
        {
            _context = context;
        }

        // 1. Get All Quizze
        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _context.Quizze
                .Include(q => q.Doctor) // Eagerly load the related Doctor
                .Include(q => q.Questions) // Include Question
                .ThenInclude(q => q.Options) // Include options for each question
                .ToListAsync();
        }

        // 2. Get Quiz by ID
        public async Task<Quiz> GetQuizByIdAsync(int quizId)
        {
            return await _context.Quizze
                .Include(q => q.Doctor)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        // 3. Get Quizze by Doctor ID
        public async Task<IEnumerable<Quiz>> GetQuizzesByDoctorIdAsync(string doctorId)
        {
            return await _context.Quizze
                .Where(q => q.DoctorId.ToString() == doctorId)
                .Include(q => q.Questions)
                .ToListAsync();
        }

        // 4. Create a New Quiz
        public async Task<Quiz> CreateQuizAsync(QuizCreationDTO quizDto)
        {
            var doctor = await _context.Doctors.FindAsync(quizDto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");

            // Create a new Quiz object
            var quiz = new Quiz
            {
                Title = quizDto.Title,
                Description = quizDto.Description,
                DoctorId = doctor.DoctorId
            };
            _context.Quizze.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        // 5. Delete a Quiz
        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            var quiz = await _context.Quizze.FindAsync(quizId);
            if (quiz == null)
                return false;

            _context.Quizze.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        // 6. Add Question to a Quiz
        public async Task<bool> AddQuestionsToQuizAsync(int quizId, List<QuestionDTO> questions)
        {
            // Find the quiz
            var quiz = await _context.Quizze.FindAsync(quizId);
            if (quiz == null)
                return false;

            // Add each question from the DTO
            foreach (var questionDto in questions)
            {
                var question = new Question
                {
                    Text = questionDto.Text,
                    QuizId = quizId,
                    Options = questionDto.Options.Select((optionText, index) => new Option
                    {
                        Text = optionText,
                        IsCorrect = index == questionDto.CorrectOptionIndex,
                        Score = questionDto.Score
                    }).ToList()
                };

                _context.Question.Add(question);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
            return true;
        }


        // 7. Get Quiz Results for a Patient
        public async Task<IEnumerable<QuizResult>> GetQuizResultsByPatientIdAsync(int patientId)
        {
            return await _context.QuizResult
                .Where(r => r.PatientId == patientId)
                .Include(r => r.Quiz)
                .ToListAsync();
        }

        // 8. Submit Quiz Results
        public async Task<QuizResult> SubmitQuizResultAsync(QuizResult quizResult)
        {
            _context.QuizResult.Add(quizResult);
            await _context.SaveChangesAsync();
            return quizResult;
        }

        public async Task<QuizResult> SubmitQuizAsync(QuizSubmissionDTO submission)
        {
            // Validate the Quiz and Patient
            var quiz = await _context.Quizze
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == submission.QuizId);

            if (quiz == null)
                throw new KeyNotFoundException("Quiz not found");

            var patient = await _context.Patients.FindAsync(submission.PatientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            // Calculate the score
            int totalScore = 0;
            foreach (var answer in submission.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (question == null)
                    throw new KeyNotFoundException($"Question with ID {answer.QuestionId} not found in the quiz");

                var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);
                if (selectedOption != null && selectedOption.IsCorrect)
                    totalScore++;
            }

            // Generate feedback based on the score
            string feedback = totalScore switch
            {
                var score when score == quiz.Questions.Count => "Excellent! You answered all questions correctly.",
                var score when score >= quiz.Questions.Count / 2 => "Good job! Keep practicing to improve.",
                _ => "You can do better. Review the material and try again."
            };

            // Save the result
            var quizResult = new QuizResult
            {
                QuizId = submission.QuizId,
                PatientId = submission.PatientId,
                TotalScore = totalScore,
                Feedback = feedback,
                DateTaken = DateTime.Now
            };

            _context.QuizResult.Add(quizResult);
            await _context.SaveChangesAsync();

            return quizResult;
        }

    }
}
