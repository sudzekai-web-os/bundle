using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;

namespace bundle.Test.Plugin
{
    public class LogEntryDescriptorBuilderExtensionsTests
    {
        [Fact]
        public void ToDescriptor_MapsLogLevelMessageAndCategory()
        {
            var logEntry = new LogEntry<int>(
                LogLevel.Warning,
                "Sudzekai.WebOs.Plugin.Handler",
                new EventId(42, "HandlerEvent"),
                7,
                new InvalidOperationException("boom"),
                (state, exception) => $"state={state}; error={exception?.Message}");

            var descriptor = logEntry.ToDescriptor();

            try
            {
                Assert.Equal((int)LogLevel.Warning, descriptor.LogLevel);

                Assert.Equal(
                    "state=7; error=boom",
                    AllocationUtilities.ToString(descriptor.Message, descriptor.MessageLength));

                Assert.Equal(
                    System.Text.Encoding.UTF8.GetByteCount("state=7; error=boom"),
                    descriptor.MessageLength);

                Assert.Equal(
                    "Sudzekai.WebOs.Plugin.Handler",
                    AllocationUtilities.ToString(descriptor.Category, descriptor.CategoryLength));

                Assert.Equal(
                    System.Text.Encoding.UTF8.GetByteCount("Sudzekai.WebOs.Plugin.Handler"),
                    descriptor.CategoryLength);
            }
            finally
            {
                descriptor.Dispose();
            }
        }
    }
}
