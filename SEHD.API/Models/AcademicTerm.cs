// Models/AcademicTerm.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEHD.API.Models
{
	[Table("AcademicTerms", Schema = "sehd")]
	public class AcademicTerm
	{
		[Key]
		public int TermID { get; set; }

		[Required]
		[StringLength(20)]
		public string TermCode { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string TermName { get; set; } = string.Empty;

		public int TermYear { get; set; }

		[Required]
		[StringLength(10)]
		public string TermSeason { get; set; } = string.Empty; // Spring, Summer, Fall

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		// Navigation properties
		public virtual ICollection<AdmissionData> AdmissionData { get; set; } = new List<AdmissionData>();

		public virtual ICollection<EnrollmentGoal> EnrollmentGoals { get; set; } = new List<EnrollmentGoal>();
	}
}
