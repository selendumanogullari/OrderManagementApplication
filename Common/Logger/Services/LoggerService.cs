using Serilog;

namespace Logging.Services
{
    public static class LoggerService
    {
        public static ILogger CreateLogger()
        {
            var logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OrderManagementApplicationLogs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var fileName = $"log_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";

            return new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logDirectory, fileName), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
