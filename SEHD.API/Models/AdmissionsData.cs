// Models/AdmissionData.cs (Updated)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEHD.API.Models
{
    [Table("AdmissionsData", Schema = "sehd")]
    public class AdmissionData
    {
        [Key]
        public int AdmissionID { get; set; }

        public int TermID { get; set; }

        public int ProgramID { get; set; }

        [Required]
        [StringLength(20)]
        public string AcademicCareer { get; set; } = string.Empty; // Undergraduate, Graduate

        [Required]
        [StringLength(20)]
        public string AdmitType { get; set; } = string.Empty; // New Student, Transfer Student

        public int TotalApplied { get; set; }

        public int TotalAdmitted { get; set; }

        public int TotalDenied { get; set; }

        public int TotalGrossDeposited { get; set; }

        public int TotalNetDeposited { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TermID")]
        public virtual AcademicTerm Term { get; set; } = null!;

        [ForeignKey("ProgramID")]
        public virtual AcademicProgram Program { get; set; } = null!;

        // Computed properties for frontend compatibility
        [NotMapped]
        public string AcademicCareerDescription => AcademicCareer;

        [NotMapped]
        public string AcademicPlanCode => Program?.ProgramCode ?? string.Empty;

        [NotMapped]
        public string AcademicPlanDescription => Program?.ProgramDescription ?? string.Empty;

        [NotMapped]
        public string AdmitTypeDescription => AdmitType;

        [NotMapped]
        public string Department => Program?.Department?.DepartmentCode ?? string.Empty;

        [NotMapped]
        public string ProgramType => Program?.ProgramType ?? string.Empty;
    }
}
