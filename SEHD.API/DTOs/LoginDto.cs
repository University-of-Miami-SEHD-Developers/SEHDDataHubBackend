// DTOs/LoginDto.cs

using System.ComponentModel.DataAnnotations;

namespace SEHD.API.DTOs
{
	public class LoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}
}
