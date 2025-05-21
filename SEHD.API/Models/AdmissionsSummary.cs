
namespace SEHD.API.Models
{
    public class AdmissionSummary
    {
        public int Id { get; set; }
        public int TotalApplied { get; set; }
        public int TotalAdmitted { get; set; }
        public int TotalDenied { get; set; }
        public int TotalGrossDeposited { get; set; }
        public int TotalNetDeposited { get; set; }
        public double AdmissionRate { get; set; }
        public double DenialRate { get; set; }
        public double DepositRate { get; set; }
        public string? Term { get; set; }
        public string? Department { get; set; }
        public string? Program { get; set; }
    }
}