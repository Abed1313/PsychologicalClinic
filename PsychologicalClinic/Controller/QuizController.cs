using Microsoft.AspNetCore.Mvc;
using PsychologicalClinic.Repository.Services;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using PsychologicalClinic.Models.DTO;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuiz _quiz;

    public QuizController(IQuiz quizService)
    {
        _quiz = quizService;
    }

    // 1. Get All Quizzes
    [HttpGet("GetAllQuizzes")]
    public async Task<IActionResult> GetAllQuizzes()
    {
        var quizzes = await _quiz.GetAllQuizzesAsync();
        return Ok(quizzes);
    }

    // 2. Get Quiz by ID
    [HttpGet("GetQuizById/{quizId}")]
    public async Task<IActionResult> GetQuizById(int quizId)
    {
        var quiz = await _quiz.GetQuizByIdAsync(quizId);
        if (quiz == null)
            return NotFound("Quiz not found");

        return Ok(quiz);
    }

    // 3. Get Quizzes by Doctor ID
    [HttpGet("GetQuizzesByDoctorId/{doctorId}")]
    public async Task<IActionResult> GetQuizzesByDoctorId(string doctorId)
    {
        var quizzes = await _quiz.GetQuizzesByDoctorIdAsync(doctorId);
        return Ok(quizzes);
    }

    // 4. Create a New Quiz
    [HttpPost("CreateQuiz")]
    public async Task<IActionResult> CreateQuiz(QuizCreationDTO quiz)
    {
        if (quiz == null)
            return BadRequest("Quiz data is required");

        var createdQuiz = await _quiz.CreateQuizAsync(quiz);
        return Ok(new { message = "Quiz created successfully", QuizId = createdQuiz.QuizId });
    }

    // 5. Delete a Quiz
    [HttpDelete("DeleteQuiz/{quizId}")]
    public async Task<IActionResult> DeleteQuiz(int quizId)
    {
        var success = await _quiz.DeleteQuizAsync(quizId);
        if (!success)
            return NotFound("Quiz not found or already deleted");

        return Ok("Quiz deleted successfully");
    }

    // 6. Add Questions to a Quiz
    [HttpPost("AddQuestions/{quizId}")]
    public async Task<IActionResult> AddQuestions(int quizId, List<QuestionDTO> questions)
    {
        if (questions == null || questions.Count == 0)
            return BadRequest("Questions data is required");

        var success = await _quiz.AddQuestionsToQuizAsync(quizId, questions);
        if (!success)
            return NotFound("Quiz not found");

        return Ok("Questions added successfully");
    }

    // 7. Get Quiz Results for a Patient
    [HttpGet("GetQuizResultsByPatientId/{patientId}")]
    public async Task<IActionResult> GetQuizResultsByPatientId(int patientId)
    {
        var results = await _quiz.GetQuizResultsByPatientIdAsync(patientId);
        return Ok(results);
    }

    // 8. Submit Quiz Results
    [HttpPost("SubmitQuizResult")]
    public async Task<IActionResult> SubmitQuizResult([FromBody] QuizResult quizResult)
    {
        if (quizResult == null)
            return BadRequest("QuizResult data is required");

        var result = await _quiz.SubmitQuizResultAsync(quizResult);
        return Ok(new { message = "Quiz result submitted successfully", ResultId = result.QuizResultId });
    }

    [HttpPost("SubmitQuiz")]
    public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionDTO submission)
    {
        var result = await _quiz.SubmitQuizAsync(submission);
        return Ok(new
        {
            message = "Quiz submitted successfully",
            result.TotalScore,
            result.Feedback,
            result.DateTaken
        });
    }
}