// Controllers/AdmissionsDataController.cs (Enhanced with debugging)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEHD.API.Data;
using SEHD.API.DTOs;
using SEHD.API.Models;
using SEHD.API.Services;

namespace SEHD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdmissionsDataController : ControllerBase
    {
        private readonly IAdmissionService _admissionService;
        private readonly SEHDDbContext _context;
        private readonly ILogger<AdmissionsDataController> _logger;

        public AdmissionsDataController(
            IAdmissionService admissionService,
            SEHDDbContext context,
            ILogger<AdmissionsDataController> logger)
        {
            _admissionService = admissionService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Debug endpoint to check what's in the database
        /// </summary>
        [HttpGet("debug")]
        public async Task<ActionResult<object>> GetDebugInfo()
        {
            try
            {
                var termCount = await _context.AcademicTerms.CountAsync();
                var programCount = await _context.AcademicPrograms.CountAsync();
                var admissionCount = await _context.AdmissionsData.CountAsync();
                var deptCount = await _context.Departments.CountAsync();

                var terms = await _context.AcademicTerms
                    .Select(t => new { t.TermCode, t.TermName, t.TermYear })
                    .ToListAsync();

                var admissionSample = await _context.AdmissionsData
                    .Include(a => a.Term)
                    .Include(a => a.Program)
                    .Take(5)
                    .Select(a => new {
                        a.AdmissionID,
                        TermCode = a.Term.TermCode,
                        ProgramCode = a.Program.ProgramCode,
                        a.TotalApplied,
                        a.TotalAdmitted
                    })
                    .ToListAsync();

                return Ok(new
                {
                    DatabaseCounts = new
                    {
                        Terms = termCount,
                        Programs = programCount,
                        Admissions = admissionCount,
                        Departments = deptCount
                    },
                    AvailableTerms = terms,
                    SampleAdmissionData = admissionSample
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug endpoint");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all admission data
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> GetAllAdmissionData()
        {
            try
            {
                _logger.LogInformation("Getting all admission data");
                var data = await _admissionService.GetAllAdmissionDataAsync();
                _logger.LogInformation($"Found {data.Count()} admission records");
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all admission data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get admission data by term - Enhanced with debugging
        /// </summary>
        [HttpGet("term/{termCode}")]
        public async Task<ActionResult<IEnumerable<AdmissionDataDto>>> GetAdmissionDataByTerm(string termCode)
        {
            try
            {
                _logger.LogInformation($"Searching for admission data with term code: '{termCode}'");

                // First, check if the term exists
                var term = await _context.AcademicTerms
                    .FirstOrDefaultAsync(t => t.TermCode == termCode);

                if (term == null)
                {
                    _logger.LogWarning($"Term '{termCode}' not found in database");

                    // Get all available terms for debugging
                    var availableTerms = await _context.AcademicTerms
                        .Select(t => t.TermCode)
                        .ToListAsync();

                    return NotFound(new
                    {
                        message = $"Term '{termCode}' not found",
                        availableTerms = availableTerms
                    });
                }

                _logger.LogInformation($"Found term: {term.TermName} (ID: {term.TermID})");

                // Check if there's admission data for this term
                var admissionCount = await _context.AdmissionsData
                    .Where(a => a.TermID == term.TermID)
                    .CountAsync();

                _logger.LogInformation($"Found {admissionCount} admission records for term {termCode}");

                if (admissionCount == 0)
                {
                    return Ok(new List<AdmissionDataDto>()); // Return empty list instead of 404
                }

                var data = await _admissionService.GetAdmissionDataByTermAsync(termCode);
                _logger.LogInformation($"Successfully retrieved {data.Count()} admission records for term {termCode}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admission data for term: {TermCode}", termCode);
                return StatusCode(500, $"Internal server error: {ex.Message}");
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