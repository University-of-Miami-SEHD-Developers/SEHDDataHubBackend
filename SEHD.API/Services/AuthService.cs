// Services/AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SEHD.API.Data;
using SEHD.API.DTOs;

namespace SEHD.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly SEHDDbContext _context;
		private readonly IConfiguration _configuration;

		public AuthService(SEHDDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<UserDto?> LoginAsync(LoginDto loginDto)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

			if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
				return null;

			await UpdateLastLoginAsync(user.UserID);

			return new UserDto
			{
				UserID = user.UserID,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = user.Role,
				IsActive = user.IsActive,
				LastLogin = user.LastLogin,
				FullName = user.FullName
			};
		}

		public async Task<string> GenerateJwtTokenAsync(UserDto user)
		{
            await Task.CompletedTask;

            var jwtSettings = _configuration.GetSection("JwtSettings");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.FullName),
				new Claim(ClaimTypes.Role, user.Role)
			};

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpiryDays"])),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<UserDto?> GetUserByEmailAsync(string email)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

			if (user == null)
				return null;

			return new UserDto
			{
				UserID = user.UserID,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = user.Role,
				IsActive = user.IsActive,
				LastLogin = user.LastLogin,
				FullName = user.FullName
			};
		}

		public async Task UpdateLastLoginAsync(int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				user.LastLogin = DateTime.UtcNow;
				user.ModifiedDate = DateTime.UtcNow;
				await _context.SaveChangesAsync();
			}
		}

		private static bool VerifyPassword(string password, string hash)
		{
			// For demo purposes, using simple hash comparison
			// In production, use proper password hashing like BCrypt
			return HashPassword(password) == hash;
		}

		public static string HashPassword(string password)
		{
			// Simple hash for demo - use BCrypt or similar in production
			using var sha256 = SHA256.Create();
			var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "sehd_salt"));
			return Convert.ToBase64String(hashedBytes);
		}
	}
}