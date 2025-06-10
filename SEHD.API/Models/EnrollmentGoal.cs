// Models/EnrollmentGoal.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEHD.API.Models
{
    [Table("EnrollmentGoals", Schema = "sehd")]
    public class EnrollmentGoal
    {
        [Key]
        public int GoalID { get; set; }

        public int ProgramID { get; set; }

        public int TermID { get; set; }

        public int GoalYear { get; set; }

        public int? TargetEnrollment { get; set; }

        public int? ActualEnrollment { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProgramID")]
        public virtual AcademicProgram Program { get; set; } = null!;

        [ForeignKey("TermID")]
        public virtual AcademicTerm Term { get; set; } = null!;
    }
}