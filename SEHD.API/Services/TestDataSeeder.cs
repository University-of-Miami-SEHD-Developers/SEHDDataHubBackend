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
        private readonly ILogger<TestDataSeeder> _logger;

        public TestDataSeeder(SEHDDbContext context, ILogger<TestDataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedTestDataAsync()
        {
            try
            {
                _logger.LogInformation("Starting test data seeding...");

                // Seed Departments (only if they don't exist)
                if (!await _context.Departments.AnyAsync())
                {
                    _logger.LogInformation("Seeding departments...");
                    var departments = new[]
                    {
                        new Department { DepartmentCode = "KIN", DepartmentName = "Kinesiology" },
                        new Department { DepartmentCode = "EPS", DepartmentName = "Educational & Psychological Studies" },
                        new Department { DepartmentCode = "TAL", DepartmentName = "Teaching and Learning" },
                        new Department { DepartmentCode = "Undeclared", DepartmentName = "Undeclared" }
                    };

                    _context.Departments.AddRange(departments);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {departments.Length} departments");
                }

                // Seed Academic Terms (only if they don't exist)
                if (!await _context.AcademicTerms.AnyAsync())
                {
                    _logger.LogInformation("Seeding academic terms...");
                    var terms = new[]
                    {
                        new AcademicTerm { TermCode = "Spring22", TermName = "Spring 2022", TermYear = 2022, TermSeason = "Spring" },
                        new AcademicTerm { TermCode = "Summer22", TermName = "Summer 2022", TermYear = 2022, TermSeason = "Summer" },
                        new AcademicTerm { TermCode = "Fall22", TermName = "Fall 2022", TermYear = 2022, TermSeason = "Fall" },
                        new AcademicTerm { TermCode = "Spring23", TermName = "Spring 2023", TermYear = 2023, TermSeason = "Spring" },
                        new AcademicTerm { TermCode = "Summer23", TermName = "Summer 2023", TermYear = 2023, TermSeason = "Summer" },
                        new AcademicTerm { TermCode = "Fall23", TermName = "Fall 2023", TermYear = 2023, TermSeason = "Fall" },
                        new AcademicTerm { TermCode = "Spring24", TermName = "Spring 2024", TermYear = 2024, TermSeason = "Spring" },
                        new AcademicTerm { TermCode = "Summer24", TermName = "Summer 2024", TermYear = 2024, TermSeason = "Summer" },
                        new AcademicTerm { TermCode = "Fall24", TermName = "Fall 2024", TermYear = 2024, TermSeason = "Fall" }
                    };

                    _context.AcademicTerms.AddRange(terms);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {terms.Length} academic terms");
                }

                // Seed Academic Programs (only if they don't exist)
                if (!await _context.AcademicPrograms.AnyAsync())
                {
                    _logger.LogInformation("Seeding academic programs...");

                    var departments = await _context.Departments.ToListAsync();
                    var kinDept = departments.First(d => d.DepartmentCode == "KIN");
                    var epsDept = departments.First(d => d.DepartmentCode == "EPS");
                    var talDept = departments.First(d => d.DepartmentCode == "TAL");
                    var undeclaredDept = departments.First(d => d.DepartmentCode == "Undeclared");

                    var programs = new[]
                    {
                        // KIN Programs
                        new AcademicProgram
                        {
                            ProgramCode = "EXPS_BSEXP",
                            ProgramDescription = "Exercise Physiology",
                            ProgramType = "Bachelor's",
                            DepartmentID = kinDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "SADM_BSED",
                            ProgramDescription = "Sport Administration",
                            ProgramType = "Bachelor's",
                            DepartmentID = kinDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "APPH_MSED",
                            ProgramDescription = "Applied Physiology",
                            ProgramType = "Master's",
                            DepartmentID = kinDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "SADM_MSED",
                            ProgramDescription = "Sport Administration",
                            ProgramType = "Master's",
                            DepartmentID = kinDept.DepartmentID
                        },

                        // EPS Programs
                        new AcademicProgram
                        {
                            ProgramCode = "CAPS_BSED",
                            ProgramDescription = "Community&AppliedPsych Studies",
                            ProgramType = "Bachelor's",
                            DepartmentID = epsDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "DASI_BS",
                            ProgramDescription = "Data Analytics Social Impact",
                            ProgramType = "Bachelor's",
                            DepartmentID = epsDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "CNSM_MSED",
                            ProgramDescription = "Counseling Mental Health",
                            ProgramType = "Master's",
                            DepartmentID = epsDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "ODAPE_MS",
                            ProgramDescription = "Data Analytics & Program Eval",
                            ProgramType = "Master's",
                            DepartmentID = epsDept.DepartmentID
                        },

                        // TAL Programs
                        new AcademicProgram
                        {
                            ProgramCode = "ELEDS_BSED",
                            ProgramDescription = "Elementary Ed Special Ed",
                            ProgramType = "Bachelor's",
                            DepartmentID = talDept.DepartmentID
                        },
                        new AcademicProgram
                        {
                            ProgramCode = "SPED_MSED",
                            ProgramDescription = "Special Education",
                            ProgramType = "Master's",
                            DepartmentID = talDept.DepartmentID
                        },

                        // Undeclared
                        new AcademicProgram
                        {
                            ProgramCode = "ED_BSED_UN",
                            ProgramDescription = "Undeclared Education",
                            ProgramType = "Bachelor's",
                            DepartmentID = undeclaredDept.DepartmentID
                        }
                    };

                    _context.AcademicPrograms.AddRange(programs);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {programs.Length} academic programs");
                }

                // ALWAYS seed admission data if it doesn't exist
                if (!await _context.AdmissionsData.AnyAsync())
                {
                    _logger.LogInformation("Seeding admission data...");

                    var terms = await _context.AcademicTerms.ToListAsync();
                    var programs = await _context.AcademicPrograms.ToListAsync();

                    var fall24Term = terms.First(t => t.TermCode == "Fall24");
                    var expsProgram = programs.First(p => p.ProgramCode == "EXPS_BSEXP");
                    var sadmProgram = programs.First(p => p.ProgramCode == "SADM_BSED");
                    var capsProgram = programs.First(p => p.ProgramCode == "CAPS_BSED");
                    var eledsProgram = programs.First(p => p.ProgramCode == "ELEDS_BSED");
                    var dasiProgram = programs.First(p => p.ProgramCode == "DASI_BS");
                    var undeclaredProgram = programs.First(p => p.ProgramCode == "ED_BSED_UN");

                    var fall24AdmissionData = new[]
                    {
                        // Exercise Physiology - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = expsProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 595,
                            TotalAdmitted = 124,
                            TotalDenied = 204,
                            TotalGrossDeposited = 41,
                            TotalNetDeposited = 38
                        },
                        // Exercise Physiology - Transfer Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = expsProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "Transfer Student",
                            TotalApplied = 38,
                            TotalAdmitted = 19,
                            TotalDenied = 0,
                            TotalGrossDeposited = 12,
                            TotalNetDeposited = 9
                        },
                        // Sport Administration - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = sadmProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 615,
                            TotalAdmitted = 118,
                            TotalDenied = 290,
                            TotalGrossDeposited = 46,
                            TotalNetDeposited = 39
                        },
                        // Sport Administration - Transfer Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = sadmProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "Transfer Student",
                            TotalApplied = 50,
                            TotalAdmitted = 20,
                            TotalDenied = 1,
                            TotalGrossDeposited = 13,
                            TotalNetDeposited = 11
                        },
                        // Community & Applied Psych Studies - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = capsProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 346,
                            TotalAdmitted = 39,
                            TotalDenied = 154,
                            TotalGrossDeposited = 13,
                            TotalNetDeposited = 12
                        },
                        // Community & Applied Psych Studies - Transfer Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = capsProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "Transfer Student",
                            TotalApplied = 14,
                            TotalAdmitted = 1,
                            TotalDenied = 1,
                            TotalGrossDeposited = 1,
                            TotalNetDeposited = 1
                        },
                        // Data Analytics Social Impact - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = dasiProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 31,
                            TotalAdmitted = 12,
                            TotalDenied = 8,
                            TotalGrossDeposited = 8,
                            TotalNetDeposited = 6
                        },
                        // Elementary Ed Special Ed - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = eledsProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 423,
                            TotalAdmitted = 52,
                            TotalDenied = 183,
                            TotalGrossDeposited = 17,
                            TotalNetDeposited = 14
                        },
                        // Undeclared Education - New Student
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = undeclaredProgram.ProgramID,
                            AcademicCareer = "Undergraduate",
                            AdmitType = "New Student",
                            TotalApplied = 132,
                            TotalAdmitted = 13,
                            TotalDenied = 56,
                            TotalGrossDeposited = 3,
                            TotalNetDeposited = 3
                        }
                    };

                    _context.AdmissionsData.AddRange(fall24AdmissionData);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {fall24AdmissionData.Length} Fall24 admission records");

                    // Add some Master's program data too
                    var sadmMasterProgram = programs.First(p => p.ProgramCode == "SADM_MSED");
                    var cnsmProgram = programs.First(p => p.ProgramCode == "CNSM_MSED");
                    var odapeProgram = programs.First(p => p.ProgramCode == "ODAPE_MS");

                    var fall24MasterData = new[]
                    {
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = sadmMasterProgram.ProgramID,
                            AcademicCareer = "Graduate",
                            AdmitType = "New Student",
                            TotalApplied = 110,
                            TotalAdmitted = 70,
                            TotalDenied = 8,
                            TotalGrossDeposited = 36,
                            TotalNetDeposited = 27
                        },
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = cnsmProgram.ProgramID,
                            AcademicCareer = "Graduate",
                            AdmitType = "New Student",
                            TotalApplied = 118,
                            TotalAdmitted = 70,
                            TotalDenied = 19,
                            TotalGrossDeposited = 39,
                            TotalNetDeposited = 35
                        },
                        new AdmissionData
                        {
                            TermID = fall24Term.TermID,
                            ProgramID = odapeProgram.ProgramID,
                            AcademicCareer = "Graduate",
                            AdmitType = "New Student",
                            TotalApplied = 42,
                            TotalAdmitted = 23,
                            TotalDenied = 3,
                            TotalGrossDeposited = 19,
                            TotalNetDeposited = 13
                        }
                    };

                    _context.AdmissionsData.AddRange(fall24MasterData);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Seeded {fall24MasterData.Length} additional Fall24 master's admission records");
                }
                else
                {
                    var admissionCount = await _context.AdmissionsData.CountAsync();
                    _logger.LogInformation($"Admission data already exists: {admissionCount} records");
                }

                // Seed Test Users (only if they don't exist)
                if (!await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding test users...");
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
                    _logger.LogInformation($"Seeded {users.Length} test users");
                }

                // Final count log
                var finalCounts = new
                {
                    Departments = await _context.Departments.CountAsync(),
                    Terms = await _context.AcademicTerms.CountAsync(),
                    Programs = await _context.AcademicPrograms.CountAsync(),
                    Admissions = await _context.AdmissionsData.CountAsync(),
                    Users = await _context.Users.CountAsync()
                };

                _logger.LogInformation($"Final database counts: {finalCounts.Departments} departments, {finalCounts.Terms} terms, {finalCounts.Programs} programs, {finalCounts.Admissions} admission records, {finalCounts.Users} users");
                _logger.LogInformation("Test data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during test data seeding");
                throw;
            }
        }
    }
}