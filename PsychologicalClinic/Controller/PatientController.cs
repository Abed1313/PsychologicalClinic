using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PsychologicalClinic.Data;
using PsychologicalClinic.Models;
using PsychologicalClinic.Repository.Interface;
using PsychologicalClinic.Repository.Services;

namespace PsychologicalClinic.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatient _patientService;

        public PatientController(IPatient context)
        {
            _patientService = context;
        }

        // GET : api/Patient/GetPatientById
        [HttpGet("GetPatientById/{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(string Id)
        {
            return await _patientService.GetPatientById(Id);
        }

        // PUT : api/Patient/EditPatientData
        [HttpPut("EditPatientData")]
        public async Task<ActionResult<Patient>> PutUser(Patient patient)
        {
            var updatedUser = await _patientService.EditPatientData(patient);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return updatedUser;
        }

        // POST: api/Patient/likevideo/5
        [HttpPost("likevideo/{videoId}")]
        public async Task<IActionResult> LikeVideoAsync(int videoId)
        {
            try
            {
                await _patientService.LikeVideoAsync(videoId);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound($"Video with ID {videoId} not found.");
            }
        }

        // POST: api/Patient/AddComment
        [HttpPost("AddComment")]
        public async Task<PatientComment> AddComment(PatientComment comment)
        {
            return await _patientService.AddComment(comment);
        }
    }
}