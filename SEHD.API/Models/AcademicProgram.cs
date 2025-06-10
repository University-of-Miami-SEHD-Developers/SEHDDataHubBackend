// Models/AcademicProgram.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEHD.API.Models
{
    [Table("AcademicPrograms", Schema = "sehd")]
    public class AcademicProgram
    {
        [Key]
        public int ProgramID { get; set; }

        [Required]
        [StringLength(20)]
        public string ProgramCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProgramDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ProgramType { get; set; } = string.Empty; // Bachelor's, Master's, Doctoral, Certificate

        public int DepartmentID { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("DepartmentID")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<AdmissionData> AdmissionData { get; set; } = new List<AdmissionData>();

        public virtual ICollection<EnrollmentGoal> EnrollmentGoals { get; set; } = new List<EnrollmentGoal>();
    }
}