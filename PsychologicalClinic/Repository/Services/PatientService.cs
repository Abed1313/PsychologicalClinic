using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace PsychologicalClinic.Repository.Services
{
    public class PatientService : IPatient
    {
        private readonly ClinicDbContext _context;
        private readonly UserManager<Patient> _userManager;

        public PatientService(ClinicDbContext clinicDbContext, UserManager<Patient> userManager)
        {
            _context = clinicDbContext;
            _userManager = userManager;
        }
        public async Task<Patient> EditPatientData(Patient user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return user;
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task LikeVideoAsync(int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null)
            {
                video.Likes++;
                _context.Videos.Update(video);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PatientComment> AddComment(PatientComment comment)
        {
            var newComment = await _context.PatientComments.AddAsync(comment);
            await _context.SaveChangesAsync();            
            return comment;
        }

        public Task<Patient> GetPatientById(string Id)
        {
            var patientById = _userManager.FindByIdAsync(Id);
            return patientById;
        }
    }
}
