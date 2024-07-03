
namespace LoggingMicroservice.Logic.Abstract
{
    public interface ILogFileProvider
    {
        /// <summary>
        /// Получить адрес до файла логами
        /// </summary>
        string? GetLogFilePath();

        /// <summary>
        /// Получить контент файла с логами
        /// </summary>
        Task<string?> GetLogFileContent(CancellationToken token);
    }
}