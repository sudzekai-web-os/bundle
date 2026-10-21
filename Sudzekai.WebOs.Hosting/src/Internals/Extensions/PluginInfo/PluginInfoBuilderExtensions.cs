using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal static class PluginInfoBuilderExtensions
    {
        extension (PluginInfo)
        {
            public static PluginInfo FromDescriptor(PluginInfoDescriptor descriptor)
            {
                return new()
                {
                    Name = AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength),
                    Version = AllocationUtilities.ToString(descriptor.Version, descriptor.VersionLength),
                    Summary = AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength)
                };
            }
        }
    }
}
