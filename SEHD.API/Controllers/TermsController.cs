// Controllers/TermsController.cs
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
    public class TermsController : ControllerBase
    {
        private readonly SEHDDbContext _context;
        private readonly ILogger<TermsController> _logger;

        public TermsController(SEHDDbContext context, ILogger<TermsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all academic terms
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicTerm>>> GetTerms()
        {
            try
            {
                var terms = await _context.AcademicTerms
                    .OrderByDescending(t => t.TermYear)
                    .ThenByDescending(t => t.TermSeason)
                    .ToListAsync();

                return Ok(terms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving terms");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get term by code
        /// </summary>
        [HttpGet("{termCode}")]
        public async Task<ActionResult<AcademicTerm>> GetTerm(string termCode)
        {
            try
            {
                var term = await _context.AcademicTerms
                    .FirstOrDefaultAsync(t => t.TermCode == termCode);

                if (term == null)
                    return NotFound();

                return Ok(term);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving term: {TermCode}", termCode);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get terms by year
        /// </summary>
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<AcademicTerm>>> GetTermsByYear(int year)
        {
            try
            {
                var terms = await _context.AcademicTerms
                    .Where(t => t.TermYear == year)
                    .OrderBy(t => t.TermSeason)
                    .ToListAsync();

                return Ok(terms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving terms for year: {Year}", year);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}