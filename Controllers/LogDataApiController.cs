using LoggingMicroservice.Logic.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingMicroservice.Controllers
{
    [ApiController]
    [Authorize(Policy ="RequireAuthenticatedUser", AuthenticationSchemes = "BasicAuthentication")]
    [Route("api/[controller]")]
    public class LogDataApiController : ControllerBase
    {
        private readonly ILogger<LogDataApiController> _logger;
        private readonly ILogFileProvider _logLogic;

        public LogDataApiController(ILogFileProvider logLogic, ILogger<LogDataApiController> logger)
        {
            _logLogic = logLogic;
            _logger = logger;
        }

        /// <summary>
        /// Получить содержимое файла логирования
        /// </summary>
        /// <returns>Содержимое файла логирования</returns>
        /// <response code="200">Возвращает содержимое файла логирования</response>
        /// <response code="404">Файл логирования не найден</response>
        /// <response code="401">Требуется авторизация</response>
        [HttpGet("Data")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Produces("text/plain")]
        public async Task<IActionResult> GetLogFileDataAsync(CancellationToken token) 
        {
            try
            {
                var logContent = await _logLogic.GetLogFileContent(token);
                if (string.IsNullOrEmpty(logContent))
                {
                    return NotFound("Log file not foun");
                }
                return Content(logContent, "text/plain");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reading log file.");
                return StatusCode(500, "Error while reading log file.");
            }
        }

        /// <summary>
        /// Получить файл логирования
        /// </summary>
        /// <returns>Содержимое файла логирования</returns>
        /// <response code="200">Возвращает содержимое файла логирования</response>
        /// <response code="404">Файл логирования не найден</response>
        /// <response code="401">Требуется авторизация</response>
        [HttpGet("File")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Produces("text/plain")]
        public IActionResult GetLogFile()
        {
            try
            {
                string? filePath = _logLogic.GetLogFilePath();
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("Log file path not found: {LogFilePath}", filePath);
                     return NotFound("Log file not foun");
                }

                FileStream fileStream = System.IO.File.OpenRead(filePath);
                string contentType = "text/plain";

                return File(fileStream, contentType, "Log");  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reading log file.");
                return StatusCode(500, "Error while reading log file.");
            }
        }
    }
}