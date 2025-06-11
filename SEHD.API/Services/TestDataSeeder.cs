using Microsoft.EntityFrameworkCore;
using SEHD.API.Data;
using SEHD.API.Models;

namespace SEHD.API.Services
{
    public interface ITestDataSeeder
    {
        Task SeedTestDataAsync();
    }

    public class TestDataSeeder : ITestDataSeeder
    {
        private readonly SEHDDbContext _context;

        public TestDataSeeder(SEHDDbContext context)
        {
            _context = context;
        }

        public async Task SeedTestDataAsync()
        {
            // Check if data already exists
            if (await _context.Departments.AnyAsync())
                return;

            // Seed Departments
            var departments = new[]
            {
                new Department { DepartmentCode = "KIN", DepartmentName = "Kinesiology" },
                new Department { DepartmentCode = "EPS", DepartmentName = "Educational & Psychological Studies" },
                new Department { DepartmentCode = "TAL", DepartmentName = "Teaching and Learning" },
                new Department { DepartmentCode = "Undeclared", DepartmentName = "Undeclared" }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Seed Academic Terms
            var terms = new[]
            {
                new AcademicTerm { TermCode = "Fall24", TermName = "Fall 2024", TermYear = 2024, TermSeason = "Fall" },
                new AcademicTerm { TermCode = "Spring24", TermName = "Spring 2024", TermYear = 2024, TermSeason = "Spring" },
                new AcademicTerm { TermCode = "Fall23", TermName = "Fall 2023", TermYear = 2023, TermSeason = "Fall" }
            };

            _context.AcademicTerms.AddRange(terms);
            await _context.SaveChangesAsync();

            // Seed Academic Programs
            var programs = new[]
            {
                new AcademicProgram
                {
                    ProgramCode = "EXPS_BSEXP",
                    ProgramDescription = "Exercise Physiology",
                    ProgramType = "Bachelor's",
                    DepartmentID = departments[0].DepartmentID
                },
                new AcademicProgram
                {
                    ProgramCode = "SADM_BSED",
                    ProgramDescription = "Sport Administration",
                    ProgramType = "Bachelor's",
                    DepartmentID = departments[0].DepartmentID
                },
                new AcademicProgram
                {
                    ProgramCode = "CAPS_BSED",
                    ProgramDescription = "Community&AppliedPsych Studies",
                    ProgramType = "Bachelor's",
                    DepartmentID = departments[1].DepartmentID
                }
            };

            _context.AcademicPrograms.AddRange(programs);
            await _context.SaveChangesAsync();

            // Seed Sample Admission Data
            var admissionData = new[]
            {
                new AdmissionData
                {
                    TermID = terms[0].TermID,
                    ProgramID = programs[0].ProgramID,
                    AcademicCareer = "Undergraduate",
                    AdmitType = "New Student",
                    TotalApplied = 595,
                    TotalAdmitted = 124,
                    TotalDenied = 204,
                    TotalGrossDeposited = 41,
                    TotalNetDeposited = 38
                },
                new AdmissionData
                {
                    TermID = terms[1].TermID,
                    ProgramID = programs[1].ProgramID,
                    AcademicCareer = "Undergraduate",
                    AdmitType = "New Student",
                    TotalApplied = 34,
                    TotalAdmitted = 28,
                    TotalDenied = 0,
                    TotalGrossDeposited = 9,
                    TotalNetDeposited = 6
                }
            };

            _context.AdmissionsData.AddRange(admissionData);
            await _context.SaveChangesAsync();

            // Seed Test Users
            var users = new[]
            {
                new User
                {
                    Email = "admin@miami.edu",
                    PasswordHash = AuthService.HashPassword("admin123"),
                    FirstName = "Test",
                    LastName = "Admin",
                    Role = "admin",
                    IsActive = true
                },
                new User
                {
                    Email = "staff@miami.edu",
                    PasswordHash = AuthService.HashPassword("staff123"),
                    FirstName = "Test",
                    LastName = "Staff",
                    Role = "staff",
                    IsActive = true
                }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    }
}