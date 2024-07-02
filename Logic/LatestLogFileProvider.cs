using LoggingMicroservice.Infrastructure.Options;
using LoggingMicroservice.Logic.Abstract;
using Microsoft.Extensions.Options;

namespace LoggingMicroservice.Logic
{
    public class LatestLogFileProider : ILogFileProvider
    {
        private readonly string _logFileDirectory;
        private readonly ILogger<LatestLogFileProider> _logger;

        public LatestLogFileProider(IOptions<LoggingInfoOptions> loggingOptions, ILogger<LatestLogFileProider> logger)
        {
            _logFileDirectory = loggingOptions.Value.LogFileDirectory;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает последний по дате лог-файл
        /// </summary>
        public async Task<string?> GetLogFileData(CancellationToken token)
        {
            string? filePath = GetLastLofFilePath();
            if (filePath == null)
            {
                _logger.LogWarning("Log file not found at path: {LogFilePath}", filePath);
                return null;
            }

            string logContent;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                logContent = await streamReader.ReadToEndAsync(token);
                _logger.LogInformation("Log file content requested");

                return logContent;
            }
        }

        public string? GetLastLofFilePath()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_logFileDirectory);
            var filePath = directoryInfo.GetFiles()
                                      .OrderByDescending(f => f.CreationTime)
                                      .FirstOrDefault()?.FullName;

            return filePath;
        }
    }
}