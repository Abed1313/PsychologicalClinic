using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Models.DTO;
using PsychologicalClinic.Models.DTO.DoctorDTO;
using PsychologicalClinic.Repository.Interface;

namespace PsychologicalClinic.Repository.Services
{
    public class DoctorService : IDoctor
    {
        private readonly ClinicDbContext _context;
        public DoctorService(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<Video> AddVideoAsync(VideoDto videoDto) // Adds a video to the doctor
        {
            // Check if the Admin exists
            var adminExists = await _context.Doctors.AnyAsync(a => a.DoctorId == videoDto.DoctorId);
            if (!adminExists)
            {
                throw new ArgumentException("Doctor with the specified ID does not exist.");
            }

            var video = new Video
            {
                Url = videoDto.Url,
                Type = videoDto.Type,
                DoctorId = videoDto.DoctorId,
                Likes = videoDto.Likes,
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task RemoveVideoAsync(int videoId) // Removes a video by ID
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null)
            {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Video> UpdateVideoAsync(VideoDto videoDto, int videoId) // Updates video details
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video == null)
            {
                throw new Exception($"Video with ID {videoId} not found.");
            }

            video.Url = videoDto.Url;
            video.Type = videoDto.Type;
            video.DoctorId = videoDto.DoctorId;
            video.Likes = videoDto.Likes;

            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<ICollection<PatientComment>> GetAllCommentsAsync(int doctorId)
        {
            return await _context.PatientComments.Where(c => c.DoctorId == doctorId).ToListAsync();
        }


        public async Task DeleteCommentAsync(int commentId) // Deletes a specific comment
        {
            var comment = await _context.PatientComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.PatientComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        // Pass DoctorId as a parameter for GetAverageRatingAsync
        public async Task<double> GetAverageRatingAsync(int doctorId)
        {
            var ratings = await _context.PatientComments
                                        .Where(c => c.DoctorId == doctorId)
                                        .Select(c => c.Rating)
                                        .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<ICollection<Patient>> GetCommentingPatientsAsync(int doctorId) // Gets patients who commented
        {
            var patientIds = await _context.PatientComments
                .Where(c => c.DoctorId == doctorId)
                .Select(c => c.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.PatientId))
                .ToListAsync();
        }

        public async Task LikeVideoAsync(int videoId) // Increments video like count
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null)
            {
                video.Likes++;
                _context.Videos.Update(video);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Disease> AddDiseaseAsync(DiseaseDto diseaseDto) // Adds a disease to expertise
        {
            // Check if the Admin exists
            var adminExistss = await _context.Doctors.AnyAsync(a => a.DoctorId == diseaseDto.DoctorId);
            if (!adminExistss)
            {
                throw new ArgumentException("Doctor with the specified ID does not exist.");
            }

            var disease = new Disease
            {
                Name = diseaseDto.Name,
                Description = diseaseDto.Description,
                DoctorId = diseaseDto.DoctorId,
            };

            _context.Diseases.Add(disease);
            await _context.SaveChangesAsync();

            return disease;
        }

        public async Task RemoveDiseaseAsync(int diseaseId) // Removes a specific disease
        {
            var disease = await _context.Diseases.FindAsync(diseaseId);
            if (disease != null)
            {
                _context.Diseases.Remove(disease);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Disease> UpdateDiseaseAsync(DiseaseDto diseaseDto , int diseaseId)
        {
            var disease = await _context.Diseases.FindAsync(diseaseId);
            if (disease == null)
            {
                throw new Exception($"Disease with ID {diseaseId} not found.");
            }

            disease.Name = diseaseDto.Name;
            disease.Description = diseaseDto.Description;
            disease.DoctorId = diseaseDto.DoctorId;

            await _context.SaveChangesAsync();
            return disease;
        }

        public async Task<Quiz> CreateQuizAsync(QuizCreationDTO quizDto)
        {
            // Find the doctor by ID
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

            // Add and save the quiz
            _context.Quizze.Add(quiz);
            await _context.SaveChangesAsync();

            return quiz;
        }

        public async Task<Quiz> UpdateQuiz(int quizId, QuizCreationDTO quizDto)
        {
            // Find the quiz by ID
            var quiz = await _context.Quizze.FindAsync(quizId);
            if (quiz == null)
                throw new KeyNotFoundException("Quiz not found");

            // Update the quiz fields
            quiz.Title = quizDto.Title;
            quiz.Description = quizDto.Description;
            _context.Quizze.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }
    }
}