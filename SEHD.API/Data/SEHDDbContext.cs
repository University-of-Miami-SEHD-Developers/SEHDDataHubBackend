// Data/SEHDDbContext.cs
using Microsoft.EntityFrameworkCore;
using SEHD.API.Models;

namespace SEHD.API.Data
{
    public class SEHDDbContext : DbContext
    {
        public SEHDDbContext(DbContextOptions<SEHDDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<AcademicProgram> AcademicPrograms { get; set; } = null!;
        public DbSet<AcademicTerm> AcademicTerms { get; set; } = null!;
        public DbSet<AdmissionData> AdmissionsData { get; set; } = null!;
        public DbSet<EnrollmentGoal> EnrollmentGoals { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentID);
                entity.HasIndex(e => e.DepartmentCode).IsUnique();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");
            });

            // Configure AcademicProgram
            modelBuilder.Entity<AcademicProgram>(entity =>
            {
                entity.HasKey(e => e.ProgramID);
                entity.HasIndex(e => e.ProgramCode).IsUnique();
                entity.HasIndex(e => e.DepartmentID);
                entity.HasIndex(e => e.ProgramType);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.AcademicPrograms)
                    .HasForeignKey(e => e.DepartmentID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure AcademicTerm
            modelBuilder.Entity<AcademicTerm>(entity =>
            {
                entity.HasKey(e => e.TermID);
                entity.HasIndex(e => e.TermCode).IsUnique();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // Configure AdmissionData
            modelBuilder.Entity<AdmissionData>(entity =>
            {
                entity.HasKey(e => e.AdmissionID);
                entity.HasIndex(e => e.TermID);
                entity.HasIndex(e => e.ProgramID);
                entity.HasIndex(e => new { e.TermID, e.ProgramID, e.AdmitType }).IsUnique()
                    .HasDatabaseName("UC_AdmissionsData");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Term)
                    .WithMany(t => t.AdmissionData)
                    .HasForeignKey(e => e.TermID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.AdmissionData)
                    .HasForeignKey(e => e.ProgramID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure EnrollmentGoal
            modelBuilder.Entity<EnrollmentGoal>(entity =>
            {
                entity.HasKey(e => e.GoalID);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Program)
                    .WithMany(p => p.EnrollmentGoals)
                    .HasForeignKey(e => e.ProgramID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Term)
                    .WithMany(t => t.EnrollmentGoals)
                    .HasForeignKey(e => e.TermID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentID = 1, DepartmentCode = "KIN", DepartmentName = "Kinesiology" },
                new Department { DepartmentID = 2, DepartmentCode = "EPS", DepartmentName = "Educational & Psychological Studies" },
                new Department { DepartmentID = 3, DepartmentCode = "TAL", DepartmentName = "Teaching and Learning" },
                new Department { DepartmentID = 4, DepartmentCode = "Undeclared", DepartmentName = "Undeclared" }
            );

            // Seed Academic Terms
            modelBuilder.Entity<AcademicTerm>().HasData(
                new AcademicTerm { TermID = 1, TermCode = "Spring22", TermName = "Spring 2022", TermYear = 2022, TermSeason = "Spring" },
                new AcademicTerm { TermID = 2, TermCode = "Summer22", TermName = "Summer 2022", TermYear = 2022, TermSeason = "Summer" },
                new AcademicTerm { TermID = 3, TermCode = "Fall22", TermName = "Fall 2022", TermYear = 2022, TermSeason = "Fall" },
                new AcademicTerm { TermID = 4, TermCode = "Spring23", TermName = "Spring 2023", TermYear = 2023, TermSeason = "Spring" },
                new AcademicTerm { TermID = 5, TermCode = "Summer23", TermName = "Summer 2023", TermYear = 2023, TermSeason = "Summer" },
                new AcademicTerm { TermID = 6, TermCode = "Fall23", TermName = "Fall 2023", TermYear = 2023, TermSeason = "Fall" },
                new AcademicTerm { TermID = 7, TermCode = "Spring24", TermName = "Spring 2024", TermYear = 2024, TermSeason = "Spring" },
                new AcademicTerm { TermID = 8, TermCode = "Summer24", TermName = "Summer 2024", TermYear = 2024, TermSeason = "Summer" },
                new AcademicTerm { TermID = 9, TermCode = "Fall24", TermName = "Fall 2024", TermYear = 2024, TermSeason = "Fall" }
            );

            // Seed some sample Academic Programs
            modelBuilder.Entity<AcademicProgram>().HasData(
                new AcademicProgram { ProgramID = 1, ProgramCode = "EXPS_BSEXP", ProgramDescription = "Exercise Physiology", ProgramType = "Bachelor's", DepartmentID = 1 },
                new AcademicProgram { ProgramID = 2, ProgramCode = "SADM_BSED", ProgramDescription = "Sport Administration", ProgramType = "Bachelor's", DepartmentID = 1 },
                new AcademicProgram { ProgramID = 3, ProgramCode = "CAPS_BSED", ProgramDescription = "Community&AppliedPsych Studies", ProgramType = "Bachelor's", DepartmentID = 2 },
                new AcademicProgram { ProgramID = 4, ProgramCode = "ELEDS_BSED", ProgramDescription = "Elementary Ed Special Ed", ProgramType = "Bachelor's", DepartmentID = 3 },
                new AcademicProgram { ProgramID = 5, ProgramCode = "DASI_BS", ProgramDescription = "Data Analytics Social Impact", ProgramType = "Bachelor's", DepartmentID = 2 },
                new AcademicProgram { ProgramID = 6, ProgramCode = "ED_BSED_UN", ProgramDescription = "Undeclared Education", ProgramType = "Bachelor's", DepartmentID = 4 }
            );
        }
    }
}