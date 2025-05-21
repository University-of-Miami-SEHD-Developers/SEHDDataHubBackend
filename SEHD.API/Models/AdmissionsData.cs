// Models/AdmissionData.cs
namespace SEHD.API.Models
{
    public class AdmissionData
    {
        public int Id { get; set; }
        public string? AcademicCareerDescription { get; set; } 
        public string? AcademicPlanCode { get; set; } 
        public string? AcademicPlanDescription { get; set; } 
        public string? AdmitTypeDescription { get; set; } 
        public string? Department { get; set; } 
        public string? Program { get; set; } 
        public int TotalApplied { get; set; }
        public int TotalAdmitted { get; set; }
        public int TotalDenied { get; set; }
        public int TotalGrossDeposited { get; set; }
        public int TotalNetDeposited { get; set; }
        public string? Term { get; set; }  // e.g., "Fall24", "Spring24"
        public string? AcademicYear { get; set; }  // e.g., "2023-24"
    }
}