// Services/AdmissionService.cs
using Microsoft.EntityFrameworkCore;
using SEHD.API.Data;
using SEHD.API.DTOs;
using SEHD.API.Models;

namespace SEHD.API.Services
{
	public class AdmissionService : IAdmissionService
	{
		private readonly SEHDDbContext _context;

		public AdmissionService(SEHDDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<AdmissionDataDto>> GetAllAdmissionDataAsync()
		{
			return await _context.AdmissionsData
				.Include(a => a.Term)
				.Include(a => a.Program)
					.ThenInclude(p => p.Department)
				.Select(a => MapToDto(a))
				.ToListAsync();
		}

		public async Task<IEnumerable<AdmissionDataDto>> GetAdmissionDataByTermAsync(string termCode)
		{
			return await _context.AdmissionsData
				.Include(a => a.Term)
				.Include(a => a.Program)
					.ThenInclude(p => p.Department)
				.Where(a => a.Term.TermCode == termCode)
				.Select(a => MapToDto(a))
				.ToListAsync();
		}

		public async Task<IEnumerable<AdmissionDataDto>> GetAdmissionDataByAcademicYearAsync(string academicYear)
		{
			// Extract year from academic year string (e.g., "2023-24" -> 2024)
			var year = int.Parse(academicYear.Split('-')[1]) + 2000;

			return await _context.AdmissionsData
				.Include(a => a.Term)
				.Include(a => a.Program)
					.ThenInclude(p => p.Department)
				.Where(a => a.Term.TermYear == year)
				.Select(a => MapToDto(a))
				.ToListAsync();
		}

		public async Task<IEnumerable<AdmissionDataDto>> FilterAdmissionDataAsync(
			string? term = null,
			string? department = null,
			string? program = null,
			string? academicCareer = null,
			string? admitType = null)
		{
			var query = _context.AdmissionsData
				.Include(a => a.Term)
				.Include(a => a.Program)
					.ThenInclude(p => p.Department)
				.AsQueryable();

			if (!string.IsNullOrEmpty(term))
				query = query.Where(a => a.Term.TermCode == term);

			if (!string.IsNullOrEmpty(department) && department != "All")
				query = query.Where(a => a.Program.Department.DepartmentCode == department);

			if (!string.IsNullOrEmpty(program) && program != "All")
				query = query.Where(a => a.Program.ProgramType == program);

			if (!string.IsNullOrEmpty(academicCareer) && academicCareer != "All")
				query = query.Where(a => a.AcademicCareer == academicCareer);

			if (!string.IsNullOrEmpty(admitType) && admitType != "All")
				query = query.Where(a => a.AdmitType == admitType);

			return await query.Select(a => MapToDto(a)).ToListAsync();
		}

		public async Task<AdmissionSummary> GetAdmissionSummaryAsync(string termCode)
		{
			var termData = await _context.AdmissionsData
				.Include(a => a.Term)
				.Where(a => a.Term.TermCode == termCode)
				.ToListAsync();

			if (!termData.Any())
				throw new ArgumentException($"No data found for term: {termCode}");

			var summary = new AdmissionSummary
			{
				Id = 1,
				TotalApplied = termData.Sum(d => d.TotalApplied),
				TotalAdmitted = termData.Sum(d => d.TotalAdmitted),
				TotalDenied = termData.Sum(d => d.TotalDenied),
				TotalGrossDeposited = termData.Sum(d => d.TotalGrossDeposited),
				TotalNetDeposited = termData.Sum(d => d.TotalNetDeposited),
				Term = termCode
			};

			summary.AdmissionRate = summary.TotalApplied > 0 ?
				(double)summary.TotalAdmitted / summary.TotalApplied * 100 : 0;

			summary.DenialRate = summary.TotalApplied > 0 ?
				(double)summary.TotalDenied / summary.TotalApplied * 100 : 0;

			summary.DepositRate = summary.TotalAdmitted > 0 ?
				(double)summary.TotalNetDeposited / summary.TotalAdmitted * 100 : 0;

			return summary;
		}

		public async Task<AdmissionData> CreateAdmissionDataAsync(AdmissionData admissionData)
		{
			_context.AdmissionsData.Add(admissionData);
			await _context.SaveChangesAsync();
			return admissionData;
		}

		public async Task<AdmissionData> UpdateAdmissionDataAsync(int id, AdmissionData admissionData)
		{
			var existing = await _context.AdmissionsData.FindAsync(id);
			if (existing == null)
				throw new ArgumentException($"Admission data with ID {id} not found");

			existing.TotalApplied = admissionData.TotalApplied;
			existing.TotalAdmitted = admissionData.TotalAdmitted;
			existing.TotalDenied = admissionData.TotalDenied;
			existing.TotalGrossDeposited = admissionData.TotalGrossDeposited;
			existing.TotalNetDeposited = admissionData.TotalNetDeposited;
			existing.ModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return existing;
		}

		public async Task<bool> DeleteAdmissionDataAsync(int id)
		{
			var admissionData = await _context.AdmissionsData.FindAsync(id);
			if (admissionData == null)
				return false;

			_context.AdmissionsData.Remove(admissionData);
			await _context.SaveChangesAsync();
			return true;
		}

		private static AdmissionDataDto MapToDto(AdmissionData admission)
		{
			return new AdmissionDataDto
			{
				Id = admission.AdmissionID,
				AcademicCareerDescription = admission.AcademicCareer,
				AcademicPlanCode = admission.Program.ProgramCode,
				AcademicPlanDescription = admission.Program.ProgramDescription,
				AdmitTypeDescription = admission.AdmitType,
				Department = admission.Program.Department.DepartmentCode,
				Program = admission.Program.ProgramType,
				TotalApplied = admission.TotalApplied,
				TotalAdmitted = admission.TotalAdmitted,
				TotalDenied = admission.TotalDenied,
				TotalGrossDeposited = admission.TotalGrossDeposited,
				TotalNetDeposited = admission.TotalNetDeposited,
				Term = admission.Term.TermCode,
				AcademicYear = $"{admission.Term.TermYear - 1}-{admission.Term.TermYear.ToString().Substring(2)}"
			};
		}
	}
}