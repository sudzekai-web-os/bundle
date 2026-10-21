using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal static class PluginInfoDescriptorBuilderExtensions
    {
        public static PluginInfoDescriptor ToDescriptor(this PluginInfo info)
        {
            var name = AllocationUtilities.AllocString(info.Name, out var nameLength);
            var version = AllocationUtilities.AllocString(info.Version, out var versionLength);
            var summary = AllocationUtilities.AllocString(info.Summary, out var summaryLength);

            return new PluginInfoDescriptor
            {
                Name = name,
                NameLength = nameLength,
                Version = version,
                VersionLength = versionLength,
                Summary = summary,
                SummaryLength = summaryLength
            };
        }
    }
}
