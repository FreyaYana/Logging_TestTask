using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingMicroservice.Controllers
{
    [ApiController]
    
    [Authorize(Policy ="RequireAuthenticatedUser", AuthenticationSchemes = "BasicAuthentication")]
    [Route("api/[controller]")]
    public class ExternalIpController : ControllerBase
    {
        private const string HeaderName = "X-Forwarded-For";
        private readonly ILogger<ExternalIpController> _logger;

        public ExternalIpController(ILogger<ExternalIpController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Получить IP-адрес клиента
        /// </summary>
        /// <returns>IP-адрес клиента</returns>
        /// <response code="200">Возвращает IP-адрес клиента</response>
        /// <response code="404">IP-адрес клиента не найден</response>
        /// <response code="401">Требуется авторизация</response>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        public ActionResult<string> GetClientIp()
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                _logger.LogInformation("Client IP Address: {IpAddress}. Method: {Method}", ipAddress, "RemoteIpAddress");
            }

            // Проверка заголовка X-Forwarded-For
            if (Request.Headers.TryGetValue(HeaderName, out var ip))
            {
                ipAddress = ip.FirstOrDefault();
                _logger.LogInformation("Client IP Address: {IpAddress}. Method: {Method}", ipAddress, HeaderName);
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                _logger.LogWarning("Client IP address not found.");
                return NotFound("Client IP address not found.");
            }

            return Ok(new { Ip = ipAddress });
        }
        
    }
}
