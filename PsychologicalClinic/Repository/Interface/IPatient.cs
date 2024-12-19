using PsychologicalClinic.Models;

namespace PsychologicalClinic.Repository.Interface
{
    public interface IPatient
    {
        public Task<Patient> EditPatientData(Patient user);
        public Task LikeVideoAsync(int videoId);
        public Task<PatientComment> AddComment(PatientComment comment);
        public Task<Patient> GetPatientById(string Id);
    }
}