
using Microsoft.AspNetCore.Mvc;
using SEHD.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace SEHD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionDataController : ControllerBase
    {
        // Fix for IDE0044: Make field readonly
        // Fix for IDE0028: Collection initialization can be simplified
        private static readonly List<AdmissionData> _admissionData = new()
        {
            new AdmissionData
            {
                Id = 1,
                AcademicCareerDescription = "Undergraduate",
                AcademicPlanCode = "EXPS_BSEXP",
                AcademicPlanDescription = "Exercise Physiology",
                AdmitTypeDescription = "New Student",
                Department = "KIN",
                Program = "Bachelor's",
                TotalApplied = 595,
                TotalAdmitted = 124,
                TotalDenied = 204,
                TotalGrossDeposited = 41,
                TotalNetDeposited = 38,
                Term = "Fall24",
                AcademicYear = "2023-24"
            },
            // Add more mock data here
        };

        [HttpGet]
        public ActionResult<IEnumerable<AdmissionData>> GetAllAdmissionData()
        {
            return Ok(_admissionData);
        }

        [HttpGet("term/{term}")]
        public ActionResult<IEnumerable<AdmissionData>> GetAdmissionDataByTerm(string term)
        {
            var data = _admissionData.Where(d => d.Term == term).ToList();
            if (!data.Any())
                return NotFound();

            return Ok(data);
        }

        [HttpGet("academic-year/{academicYear}")]
        public ActionResult<IEnumerable<AdmissionData>> GetAdmissionDataByAcademicYear(string academicYear)
        {
            var data = _admissionData.Where(d => d.AcademicYear == academicYear).ToList();
            if (!data.Any())
                return NotFound();

            return Ok(data);
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<AdmissionData>> FilterAdmissionData(
            [FromQuery] string? term = null,
            [FromQuery] string? department = null,
            [FromQuery] string? program = null,
            [FromQuery] string? academicCareer = null,
            [FromQuery] string? admitType = null)
        {
            var query = _admissionData.AsQueryable();

            if (!string.IsNullOrEmpty(term))
                query = query.Where(d => d.Term == term);

            if (!string.IsNullOrEmpty(department) && department != "All")
                query = query.Where(d => d.Department == department);

            if (!string.IsNullOrEmpty(program) && program != "All")
                query = query.Where(d => d.Program == program);

            if (!string.IsNullOrEmpty(academicCareer) && academicCareer != "All")
                query = query.Where(d => d.AcademicCareerDescription == academicCareer);

            if (!string.IsNullOrEmpty(admitType) && admitType != "All")
                query = query.Where(d => d.AdmitTypeDescription == admitType);

            var result = query.ToList();
            if (!result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpGet("summary/{term}")]
        public ActionResult<AdmissionSummary> GetAdmissionSummary(string term)
        {
            var termData = _admissionData.Where(d => d.Term == term).ToList();
            if (!termData.Any())
                return NotFound();

            var summary = new AdmissionSummary
            {
                Id = 1,
                TotalApplied = termData.Sum(d => d.TotalApplied),
                TotalAdmitted = termData.Sum(d => d.TotalAdmitted),
                TotalDenied = termData.Sum(d => d.TotalDenied),
                TotalGrossDeposited = termData.Sum(d => d.TotalGrossDeposited),
                TotalNetDeposited = termData.Sum(d => d.TotalNetDeposited),
                Term = term
            };

            summary.AdmissionRate = summary.TotalApplied > 0 ?
                (double)summary.TotalAdmitted / summary.TotalApplied * 100 : 0;

            summary.DenialRate = summary.TotalApplied > 0 ?
                (double)summary.TotalDenied / summary.TotalApplied * 100 : 0;

            summary.DepositRate = summary.TotalAdmitted > 0 ?
                (double)summary.TotalNetDeposited / summary.TotalAdmitted * 100 : 0;

            return Ok(summary);
        }
    }
}