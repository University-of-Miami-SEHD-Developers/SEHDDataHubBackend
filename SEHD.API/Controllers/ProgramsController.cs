// Controllers/ProgramsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEHD.API.Data;
using SEHD.API.Models;

namespace SEHD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgramsController : ControllerBase
    {
        private readonly SEHDDbContext _context;
        private readonly ILogger<ProgramsController> _logger;

        public ProgramsController(SEHDDbContext context, ILogger<ProgramsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all academic programs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPrograms()
        {
            try
            {
                var programs = await _context.AcademicPrograms
                    .Include(p => p.Department)
                    .Where(p => p.IsActive)
                    .Select(p => new
                    {
                        p.ProgramID,
                        p.ProgramCode,
                        p.ProgramDescription,
                        p.ProgramType,
                        Department = new
                        {
                            p.Department.DepartmentID,
                            p.Department.DepartmentCode,
                            p.Department.DepartmentName
                        }
                    })
                    .OrderBy(p => p.Department.DepartmentName)
                    .ThenBy(p => p.ProgramDescription)
                    .ToListAsync();

                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get programs by department
        /// </summary>
        [HttpGet("department/{departmentCode}")]
        public async Task<ActionResult<IEnumerable<AcademicProgram>>> GetProgramsByDepartment(string departmentCode)
        {
            try
            {
                var programs = await _context.AcademicPrograms
                    .Include(p => p.Department)
                    .Where(p => p.Department.DepartmentCode == departmentCode && p.IsActive)
                    .OrderBy(p => p.ProgramDescription)
                    .ToListAsync();

                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs for department: {DepartmentCode}", departmentCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get programs by type
        /// </summary>
        [HttpGet("type/{programType}")]
        public async Task<ActionResult<IEnumerable<AcademicProgram>>> GetProgramsByType(string programType)
        {
            try
            {
                var programs = await _context.AcademicPrograms
                    .Include(p => p.Department)
                    .Where(p => p.ProgramType == programType && p.IsActive)
                    .OrderBy(p => p.Department.DepartmentName)
                    .ThenBy(p => p.ProgramDescription)
                    .ToListAsync();

                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs for type: {ProgramType}", programType);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}