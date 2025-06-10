// Services/IAdmissionService.cs
using SEHD.API.DTOs;
using SEHD.API.Models;

namespace SEHD.API.Services
{
    public interface IAdmissionService
    {
        Task<IEnumerable<AdmissionDataDto>> GetAllAdmissionDataAsync();
        Task<IEnumerable<AdmissionDataDto>> GetAdmissionDataByTermAsync(string termCode);
        Task<IEnumerable<AdmissionDataDto>> GetAdmissionDataByAcademicYearAsync(string academicYear);
        Task<IEnumerable<AdmissionDataDto>> FilterAdmissionDataAsync(
            string? term = null,
            string? department = null,
            string? program = null,
            string? academicCareer = null,
            string? admitType = null);
        Task<AdmissionSummary> GetAdmissionSummaryAsync(string termCode);
        Task<AdmissionData> CreateAdmissionDataAsync(AdmissionData admissionData);
        Task<AdmissionData> UpdateAdmissionDataAsync(int id, AdmissionData admissionData);
        Task<bool> DeleteAdmissionDataAsync(int id);
    }
}