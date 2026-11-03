
namespace Sudzekai.LinCtrl.Core 
{
    public class LogEntry
    {
        public string Category { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public LogLevel LogLevel { get; set; }
    }
}