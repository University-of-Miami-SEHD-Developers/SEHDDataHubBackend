// DTOs/AdmissionDataDto.cs
namespace SEHD.API.DTOs
{
    public class AdmissionDataDto
    {
        public int Id { get; set; }
        public string AcademicCareerDescription { get; set; } = string.Empty;
        public string AcademicPlanCode { get; set; } = string.Empty;
        public string AcademicPlanDescription { get; set; } = string.Empty;
        public string AdmitTypeDescription { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public int TotalApplied { get; set; }
        public int TotalAdmitted { get; set; }
        public int TotalDenied { get; set; }
        public int TotalGrossDeposited { get; set; }
        public int TotalNetDeposited { get; set; }
        public string Term { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
    }
}