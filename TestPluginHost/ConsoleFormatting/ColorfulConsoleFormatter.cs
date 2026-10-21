using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace TestPluginHost.ConsoleFormatting
{
    internal class ColorfulConsoleFormatter(IOptionsMonitor<ColorfulConsoleFormatterOptions> options) : ConsoleFormatter(FormatterName)
    {
        /// <summary>
        /// The name used to identify the Toolkit console formatter.
        /// </summary>
        public const string FormatterName = "Toolkit";

        private readonly IOptionsMonitor<ColorfulConsoleFormatterOptions> _options = options;


        /// <summary>
        /// Writes a log entry to the specified text writer.
        /// </summary>
        /// <typeparam name="TState">
        /// The type of the log entry state.
        /// </typeparam>
        /// <param name="logEntry">
        /// The log entry to write.
        /// </param>
        /// <param name="scopeProvider">
        /// The provider used to access the current logging scopes.
        /// </param>
        /// <param name="textWriter">
        /// The writer to which the formatted log entry is written.
        /// </param>
        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            var options = _options.CurrentValue;

            var timestamp = DateTime.Now.ToString(options.TimestampFormat)
                .GetColoredString(options.TimestampForegroundColor)
                .GetColoredBackgroundString(options.TimestampBackgroundColor);

            string level = GetLogLevelColor(logEntry.LogLevel, options);

            var message = logEntry.Formatter(logEntry.State, logEntry.Exception)
                .GetColoredString(options.MessageForegroundColor)
                .GetColoredString(options.MessageBackgroundColor);

            var category = logEntry.Category
                .GetColoredString(options.CategoryForegroundColor)
                .GetColoredBackgroundString(options.CategoryBackgroundColor);

            textWriter.Write("{0} ", timestamp);
            textWriter.Write("{0} ", level);
            textWriter.Write("{0}", category);

            if (!options.SingleLine)
            {
                textWriter.WriteLine();
                var chars = message.ToCharArray();

                int lineLength = options.LineLength;

                for (int i = 0; i < chars.Length; i += lineLength)
                {
                    textWriter.WriteLine(
                        "{0}{1}",
                        new string(' ', 11),
                        string.Join("", chars.Skip(i).Take(lineLength)));
                }
            }
            else
            {
                textWriter.WriteLine(" {0}", message);
            }

            if (logEntry.Exception is not null)
            {
                textWriter.WriteLine(logEntry.Exception);
            }
        }

        private static string GetLogLevelColor(LogLevel logLevel, ColorfulConsoleFormatterOptions options)
            => logLevel switch
            {
                LogLevel.Trace => "TRCE".GetColoredString(options.TraceLevelForegroundColor)
                                        .GetColoredBackgroundString(options.TraceLevelBackgroundColor),

                LogLevel.Debug => "DBUG".GetColoredString(options.DebugLevelForegroundColor)
                                        .GetColoredBackgroundString(options.DebugLevelBackgroundColor),

                LogLevel.Information => "INFO".GetColoredString(options.InformationLevelForegroundColor)
                                              .GetColoredBackgroundString(options.InformationLevelBackgroundColor),

                LogLevel.Warning => "WARN".GetColoredString(options.WarningLevelForegroundColor)
                                          .GetColoredBackgroundString(options.WarningLevelBackgroundColor),

                LogLevel.Error => "ERR".GetColoredString(options.ErrorLevelForegroundColor)
                                       .GetColoredBackgroundString(options.ErrorLevelBackgroundColor),

                LogLevel.Critical => "CRIT".GetColoredString(options.CriticalLevelForegroundColor)
                                           .GetColoredBackgroundString(options.CriticalLevelBackgroundColor),
                _ => "None"
            };
    }
}
