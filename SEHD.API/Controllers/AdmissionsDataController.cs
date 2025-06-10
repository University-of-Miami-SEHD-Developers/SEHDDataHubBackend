// Controllers/AdmissionDataController.cs (Updated)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEHD.API.DTOs;
using SEHD.API.Models;
using SEHD.API.Services;

namespace SEHD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdmissionDataController : ControllerBase
    {
        private readonly IAdmissionService _admissionService;
        private readonly ILogger<AdmissionDataController> _logger;

        public AdmissionDataController(IAdmissionService admissionService, ILogger<AdmissionDataController> logger)
        {
            _admissionService = admissionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all admission data
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> GetAllAdmissionData()
        {
            try
            {
                var data = await _admissionService.GetAllAdmissionDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all admission data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get admission data by term
        /// </summary>
        [HttpGet("term/{termCode}")]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> GetAdmissionDataByTerm(string termCode)
        {
            try
            {
                var data = await _admissionService.GetAdmissionDataByTermAsync(termCode);
                if (!data.Any())
                    return NotFound($"No data found for term: {termCode}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admission data for term: {TermCode}", termCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get admission data by academic year
        /// </summary>
        [HttpGet("academic-year/{academicYear}")]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> GetAdmissionDataByAcademicYear(string academicYear)
        {
            try
            {
                var data = await _admissionService.GetAdmissionDataByAcademicYearAsync(academicYear);
                if (!data.Any())
                    return NotFound($"No data found for academic year: {academicYear}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admission data for academic year: {AcademicYear}", academicYear);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Filter admission data based on multiple criteria
        /// </summary>
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> FilterAdmissionData(
            [FromQuery] string? term = null,
            [FromQuery] string? department = null,
            [FromQuery] string? program = null,
            [FromQuery] string? academicCareer = null,
            [FromQuery] string? admitType = null)
        {
            try
            {
                var data = await _admissionService.FilterAdmissionDataAsync(term, department, program, academicCareer, admitType);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering admission data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get admission summary for a specific term
        /// </summary>
        [HttpGet("summary/{termCode}")]
        public async Task<ActionResult<AdmissionSummary>> GetAdmissionSummary(string termCode)
        {
            try
            {
                var summary = await _admissionService.GetAdmissionSummaryAsync(termCode);
                return Ok(summary);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admission summary for term: {TermCode}", termCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create new admission data entry
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<ActionResult<AdmissionData>> CreateAdmissionData([FromBody] AdmissionData admissionData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _admissionService.CreateAdmissionDataAsync(admissionData);
                return CreatedAtAction(nameof(GetAdmissionDataByTerm),
                    new { termCode = created.Term.TermCode }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admission data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update existing admission data
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<ActionResult<AdmissionData>> UpdateAdmissionData(int id, [FromBody] AdmissionData admissionData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _admissionService.UpdateAdmissionDataAsync(id, admissionData);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admission data with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete admission data
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAdmissionData(int id)
        {
            try
            {
                var deleted = await _admissionService.DeleteAdmissionDataAsync(id);
                if (!deleted)
                    return NotFound($"Admission data with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admission data with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}