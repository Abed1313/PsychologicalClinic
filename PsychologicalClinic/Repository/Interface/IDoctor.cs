using PsychologicalClinic.Models;
using PsychologicalClinic.Models.DTO.DoctorDTO;

namespace PsychologicalClinic.Repository.Interface
{
    public interface IDoctor
    {
        Task<Video> AddVideoAsync(VideoDto videoDto); // Adds a video to the doctor
        Task RemoveVideoAsync(int videoId); // Removes a video by ID
        Task<Video> UpdateVideoAsync(VideoDto videoDto, int videoId); // Updates video details
        Task<ICollection<PatientComment>> GetAllCommentsAsync(int doctorId); // Gets all patient comments
        Task DeleteCommentAsync(int commentId); // Deletes a specific comment
        Task<double> GetAverageRatingAsync(int doctorId); // Calculates average rating
        Task<ICollection<Patient>> GetCommentingPatientsAsync(int doctorId); // Gets patients who commented
        Task LikeVideoAsync(int videoId); // Increments video like count
        Task<Disease> AddDiseaseAsync(DiseaseDto diseaseDto); // Adds a disease to expertise
        Task RemoveDiseaseAsync(int diseaseId); // Removes a specific disease
        Task<Disease> UpdateDiseaseAsync(DiseaseDto diseaseDto, int diseaseId); // Updates disease details
    }
}
