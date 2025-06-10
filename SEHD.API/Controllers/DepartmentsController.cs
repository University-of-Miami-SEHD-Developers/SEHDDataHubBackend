// Controllers/DepartmentsController.cs
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
    public class DepartmentsController : ControllerBase
    {
        private readonly SEHDDbContext _context;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(SEHDDbContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all departments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            try
            {
                var departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .OrderBy(d => d.DepartmentName)
                    .ToListAsync();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get department by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.AcademicPrograms)
                    .FirstOrDefaultAsync(d => d.DepartmentID == id);

                if (department == null)
                    return NotFound();

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}