using PsychologicalClinic.Models;
using PsychologicalClinic.Models.DTO;
using PsychologicalClinic.Models.DTO.DoctorDTO;

namespace PsychologicalClinic.Repository.Interface
{
    public interface IDoctor
    {
        Task<Video> AddVideoAsync(VideoDto videoDto); // Adds a video to the doctor
        public Task RemoveVideoAsync(int videoId); // Removes a video by ID
        public Task<Video> UpdateVideoAsync(VideoDto videoDto, int videoId); // Updates video details
        Task<ICollection<PatientComment>> GetAllCommentsAsync(int doctorId); // Gets all patient comments
        public Task DeleteCommentAsync(int commentId); // Deletes a specific comment
        public Task<double> GetAverageRatingAsync(int doctorId); // Calculates average rating
        Task<ICollection<Patient>> GetCommentingPatientsAsync(int doctorId); // Gets patients who commented
        public Task LikeVideoAsync(int videoId); // Increments video like count
        public Task<Disease> AddDiseaseAsync(DiseaseDto diseaseDto); // Adds a disease to expertise
        public Task RemoveDiseaseAsync(int diseaseId); // Removes a specific disease
        public Task<Disease> UpdateDiseaseAsync(DiseaseDto diseaseDto, int diseaseId); // Updates disease details
        public Task<Quiz> CreateQuizAsync(QuizCreationDTO quizDto);
        public Task<Quiz> UpdateQuiz(int quizId, QuizCreationDTO quizDto);
    }
}