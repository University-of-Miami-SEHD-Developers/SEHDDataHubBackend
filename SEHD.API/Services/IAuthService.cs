// Services/IAuthService.cs
using SEHD.API.DTOs;

namespace SEHD.API.Services
{
    public interface IAuthService
    {
        Task<UserDto?> LoginAsync(LoginDto loginDto);
        Task<string> GenerateJwtTokenAsync(UserDto user);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task UpdateLastLoginAsync(int userId);
    }
}