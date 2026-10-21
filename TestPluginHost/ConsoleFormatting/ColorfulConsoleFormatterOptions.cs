using Microsoft.Extensions.Logging.Console;

namespace TestPluginHost.ConsoleFormatting
{
    public class ColorfulConsoleFormatterOptions : ConsoleFormatterOptions
    {
        public ColorfulConsoleFormatterOptions()
        {
            TimestampFormat = "[HH:mm:ss]";
        }

        /// <summary>
        /// Gets or sets the foreground FormatterColor for trace-level log entries.
        /// </summary>
        public FormatterColor TraceLevelForegroundColor { get; set; } = FormatterColor.Black;

        /// <summary>
        /// Gets or sets the background FormatterColor for trace-level log entries.
        /// </summary>
        public FormatterColor TraceLevelBackgroundColor { get; set; } = FormatterColor.White;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for debug-level log entries.
        /// </summary>
        public FormatterColor DebugLevelForegroundColor { get; set; } = FormatterColor.White;

        /// <summary>
        /// Gets or sets the background FormatterColor for debug-level log entries.
        /// </summary>
        public FormatterColor DebugLevelBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for information-level log entries.
        /// </summary>
        public FormatterColor InformationLevelForegroundColor { get; set; } = FormatterColor.Blue;

        /// <summary>
        /// Gets or sets the background FormatterColor for information-level log entries.
        /// </summary>
        public FormatterColor InformationLevelBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for warning-level log entries.
        /// </summary>
        public FormatterColor WarningLevelForegroundColor { get; set; } = FormatterColor.DarkYellow;

        /// <summary>
        /// Gets or sets the background FormatterColor for warning-level log entries.
        /// </summary>
        public FormatterColor WarningLevelBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for error-level log entries.
        /// </summary>
        public FormatterColor ErrorLevelForegroundColor { get; set; } = FormatterColor.Red;

        /// <summary>
        /// Gets or sets the background FormatterColor for error-level log entries.
        /// </summary>
        public FormatterColor ErrorLevelBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for critical-level log entries.
        /// </summary>
        public FormatterColor CriticalLevelForegroundColor { get; set; } = FormatterColor.White;

        /// <summary>
        /// Gets or sets the background FormatterColor for critical-level log entries.
        /// </summary>
        public FormatterColor CriticalLevelBackgroundColor { get; set; } = FormatterColor.Red;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for log messages.
        /// </summary>
        public FormatterColor MessageForegroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the background FormatterColor for log messages.
        /// </summary>
        public FormatterColor MessageBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for log categories.
        /// </summary>
        public FormatterColor CategoryForegroundColor { get; set; } = FormatterColor.DarkGray;

        /// <summary>
        /// Gets or sets the background FormatterColor for log categories.
        /// </summary>
        public FormatterColor CategoryBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the foreground FormatterColor for timestamps.
        /// </summary>
        public FormatterColor TimestampForegroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the background FormatterColor for timestamps.
        /// </summary>
        public FormatterColor TimestampBackgroundColor { get; set; } = FormatterColor.Default;

        /// <summary>
        /// Gets or sets the maximum number of characters written on a single log line.
        /// </summary>
        public int LineLength { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets a value indicating whether each log entry is written on a single line.
        /// </summary>
        public bool SingleLine { get; set; } = false;
    }
}
