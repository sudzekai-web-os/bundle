using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class LogEntryDto
    {
        public string Category { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public LogLevel LogLevel { get; set; }
    }
}
