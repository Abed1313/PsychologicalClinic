using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PsychologicalClinic.Models.DTO.DoctorDTO;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using PsychologicalClinic.Repository.Services;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Models.DTO;
using PsychologicalClinic.Data;

namespace PsychologicalClinic.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _doctorService;
        private readonly ClinicDbContext _context;


        public DoctorController(IDoctor doctorService, ClinicDbContext context)
        {
            _doctorService = doctorService;
            _context = context;
        }

        // POST: api/doctor/addvideo
        [HttpPost("addvideo")]
        public async Task<ActionResult<Video>> AddVideoAsync([FromBody] VideoDto videoDto)
        {
            if (videoDto == null)
            {
                return BadRequest("Invalid video plan data.");
            }


            var createdPlan = await _doctorService.AddVideoAsync(videoDto);

            VideoDto video = new VideoDto
            {
                Url = videoDto.Url,
                Type = videoDto.Type,
                DoctorId = videoDto.DoctorId,
                Likes = videoDto.Likes,
            };

            return Ok(video);
        }

        // DELETE: api/doctor/removevideo/5
        [HttpDelete("removevideo/{videoId}")]
        public async Task<IActionResult> RemoveVideoAsync(int videoId)
        {
            try
            {
                await _doctorService.RemoveVideoAsync(videoId);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound($"Video with ID {videoId} not found.");
            }
        }

        // PUT: api/doctor/updatevideo/5
        [HttpPut("updatevideo/{videoId}")]
        public async Task<ActionResult<Video>> UpdateVideoAsync(int videoId, [FromBody] VideoDto videoDto)
        {
            try
            {
                var updatedVideo = await _doctorService.UpdateVideoAsync(videoDto, videoId);
                return Ok(updatedVideo);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/doctor/comments/5
        [HttpGet("comments/{doctorId}")]
        public async Task<ActionResult<ICollection<PatientComment>>> GetAllCommentsAsync(int doctorId)
        {
            var comments = await _doctorService.GetAllCommentsAsync(doctorId);
            return Ok(comments);
        }

        // DELETE: api/doctor/deletecomment/5
        [HttpDelete("deletecomment/{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId)
        {
            try
            {
                await _doctorService.DeleteCommentAsync(commentId);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound($"Comment with ID {commentId} not found.");
            }
        }

        // GET: api/doctor/averagerating/5
        [HttpGet("averagerating/{doctorId}")]
        public async Task<ActionResult<double>> GetAverageRatingAsync(int doctorId)
        {
            var averageRating = await _doctorService.GetAverageRatingAsync(doctorId);
            return Ok(averageRating);
        }

        // GET: api/doctor/commentingpatients/5
        [HttpGet("commentingpatients/{doctorId}")]
        public async Task<ActionResult<ICollection<Patient>>> GetCommentingPatientsAsync(int doctorId)
        {
            var patients = await _doctorService.GetCommentingPatientsAsync(doctorId);
            return Ok(patients);
        }

        // POST: api/doctor/likevideo/5
        [HttpPost("likevideo/{videoId}")]
        public async Task<IActionResult> LikeVideoAsync(int videoId)
        {
            try
            {
                await _doctorService.LikeVideoAsync(videoId);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound($"Video with ID {videoId} not found.");
            }
        }

        // POST: api/doctor/adddisease
        [HttpPost("adddisease")]
        public async Task<ActionResult<Disease>> AddDiseaseAsync([FromBody] DiseaseDto diseaseDto)
        {
            if (diseaseDto == null)
            {
                return BadRequest("Invalid disease plan data.");
            }


            var createdPlan = await _doctorService.AddDiseaseAsync(diseaseDto);

            DiseaseDto disease = new DiseaseDto
            {
                Name = diseaseDto.Name,
                Description = diseaseDto.Description,
                DoctorId = diseaseDto.DoctorId,
            };

            return Ok(disease);
        }

        // DELETE: api/doctor/removedisease/5
        [HttpDelete("removedisease/{diseaseId}")]
        public async Task<IActionResult> RemoveDiseaseAsync(int diseaseId)
        {
            try
            {
                await _doctorService.RemoveDiseaseAsync(diseaseId);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound($"Disease with ID {diseaseId} not found.");
            }
        }

        // PUT: api/doctor/updatedisease/5
        [HttpPut("updatedisease/{diseaseId}")]
        public async Task<ActionResult<Disease>> UpdateDiseaseAsync(int diseaseId, [FromBody] DiseaseDto diseaseDto)
        {
            try
            {
                var updatedDisease = await _doctorService.UpdateDiseaseAsync(diseaseDto, diseaseId);
                return Ok(updatedDisease);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Additional methods to fetch video and disease by ID, if required for 'CreatedAtAction' or any other use
        private ActionResult<Video> GetVideoById(int videoId)
        {
            // Implement video retrieval logic based on videoId if needed
            return Ok(); // Placeholder
        }

        private ActionResult<Disease> GetDiseaseById(int diseaseId)
        {
            // Implement disease retrieval logic based on diseaseId if needed
            return Ok(); // Placeholder
        }

        [HttpPost("CreateQuiz")]
        public async Task<IActionResult> CreateQuiz(QuizCreationDTO quizDto)
        {
            try
            {
                var quiz = await _doctorService.CreateQuizAsync(quizDto);

                return Ok(new
                {
                    message = "Quiz created successfully",
                    QuizId = quiz.QuizId
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateQuiz/{quizId}")]
        public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] QuizCreationDTO quizDto)
        {
            var updatedQuiz = await _doctorService.UpdateQuiz(quizId, quizDto);
            return Ok(new { message = "Quiz updated successfully", updatedQuiz });
        }
    }
}