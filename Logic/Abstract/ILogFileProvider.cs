
namespace LoggingMicroservice.Logic.Abstract
{
    public interface ILogFileProvider
    {
        string? GetLastLofFilePath();

        Task<string?> GetLogFileData(CancellationToken token);
    }
}