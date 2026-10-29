using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;

namespace bundle.Test.Hosting
{
    public class PluginInfoBuilderExtensionsTests
    {
        [Fact]
        public void FromDescriptor_MapsNameVersionAndSummary()
        {
            var name = AllocationUtilities.AllocString("Sudzekai.Weather", out var nameLength);
            var version = AllocationUtilities.AllocString("2.4.1", out var versionLength);
            var summary = AllocationUtilities.AllocString("Weather forecast plugin", out var summaryLength);

            var descriptor = new AssemblyInfoDescriptor
            {
                Name = name,
                NameLength = nameLength,
                Version = version,
                VersionLength = versionLength,
                Summary = summary,
                SummaryLength = summaryLength
            };

            try
            {
                var info = AssemblyInfo.FromDescriptor(descriptor);

                Assert.Equal("Sudzekai.Weather", info.Name);
                Assert.Equal("2.4.1", info.Version);
                Assert.Equal("Weather forecast plugin", info.Summary);
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public void FromDescriptor_MapsEmptyValues()
        {
            var descriptor = new AssemblyInfoDescriptor();

            var info = AssemblyInfo.FromDescriptor(descriptor);

            Assert.Equal(string.Empty, info.Name);
            Assert.Equal(string.Empty, info.Version);
            Assert.Equal(string.Empty, info.Summary);
        }
    }
}
