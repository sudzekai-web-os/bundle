using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;

namespace bundle.Test.Plugin
{
    public class PluginInfoDescriptorBuilderExtensionsTests
    {
        [Fact]
        public void ToDescriptor_MapsNameVersionAndSummary()
        {
            var info = new AssemblyInfo
            {
                Name = "Sudzekai.Weather",
                Version = "2.4.1",
                Summary = "Weather forecast plugin"
            };

            var descriptor = info.ToDescriptor();

            try
            {
                Assert.Equal(
                    "Sudzekai.Weather",
                    AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength));
                Assert.Equal(
                    "2.4.1",
                    AllocationUtilities.ToString(descriptor.Version, descriptor.VersionLength));
                Assert.Equal(
                    "Weather forecast plugin",
                    AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength));
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public void ToDescriptor_MapsDefaultPluginInfo()
        {
            var descriptor = new AssemblyInfo().ToDescriptor();

            try
            {
                Assert.Equal(
                    "неизвестный плагин",
                    AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength));
                Assert.Equal(
                    "0.0.0",
                    AllocationUtilities.ToString(descriptor.Version, descriptor.VersionLength));
                Assert.Equal(
                    "описание отсутствует",
                    AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength));
            }
            finally
            {
                descriptor.Dispose();
            }
        }
    }
}
