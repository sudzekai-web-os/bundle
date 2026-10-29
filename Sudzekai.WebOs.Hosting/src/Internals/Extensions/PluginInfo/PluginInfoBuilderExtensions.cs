using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal static class PluginInfoBuilderExtensions
    {
        extension (AssemblyInfo)
        {
            public static AssemblyInfo FromDescriptor(ref AssemblyInfoDescriptor descriptor)
            {
                AssemblyInfo info = new()
                {
                    Name = AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength),
                    Version = AllocationUtilities.ToString(descriptor.Version, descriptor.VersionLength),
                    Summary = AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength)
                };

                descriptor.Dispose();

                return info;
            }
        }
    }
}
