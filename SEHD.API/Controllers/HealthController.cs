// Controllers/HealthController.cs
using Microsoft.AspNetCore.Mvc;

namespace SEHD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Simple health check endpoint
        /// </summary>
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                message = "SEHD API is running successfully!"
            });
        }

        /// <summary>
        /// Get API information
        /// </summary>
        [HttpGet("info")]
        public ActionResult<object> GetInfo()
        {
            return Ok(new
            {
                apiName = "SEHD Admissions API",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "/api/health",
                    "/api/auth/login",
                    "/api/departments",
                    "/api/programs",
                    "/api/terms",
                    "/api/admissionsdata"
                }
            });
        }
    }
}