
namespace LoggingMicroservice.Infrastructure.Options
{
    public class LoggingInfoOptions
    {
        /// <summary>
        /// Путь до папки с логами 
        /// </summary>
        public required string LogFileDirectory { get; set; }

        /// <summary>
        /// Шаблон с именем файла для логов 
        /// </summary>
        public required string LogFilePath { get; set; }

        /// <summary>
        /// Ограничение по памяти 
        /// </summary>
        public int LogFileSizeLimitBytes { get; set; }
    }
}