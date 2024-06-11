using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sora.PerformanceMaskLog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("healthz")]
        public IActionResult Healthz()
        {
            var user = new User
            {
                Username = "abc",
                Password = "123456"
            };
            _logger.LogDebug("Password=123456;username=dbhuan;");
            _logger.LogDebug($"UserSerializer={JsonConvert.SerializeObject(user)}");
            _logger.LogDebug("User={@user}", user);
            return Ok(new
            {
                Code = 200,
                Message = "Service is active"
            });
        }
    }

    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}