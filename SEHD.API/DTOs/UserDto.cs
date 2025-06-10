// DTOs/UserDto.cs
namespace SEHD.API.DTOs
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}