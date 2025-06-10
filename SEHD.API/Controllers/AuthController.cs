// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using SEHD.API.DTOs;
using SEHD.API.Services;

namespace SEHD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _authService.LoginAsync(loginDto);
                if (user == null)
                    return Unauthorized("Invalid email or password");

                var token = await _authService.GenerateJwtTokenAsync(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.UserID,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.Role,
                        user.FullName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized();

                var user = await _authService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}