using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;
using System.Text;

namespace Sudzekai.WebOs.Plugin
{
    internal static class HandlerResultDescriptorBuilderExtensions
    {
        public static HandlerResultDescriptor ToDescriptor(this HandlerResult result)
        {
            var data = AllocationUtilities.AllocString(result.Data, out var dataLength);
            return new(result.StatusCode, data, dataLength, (int)result.ContentType);
        }
    }
}
