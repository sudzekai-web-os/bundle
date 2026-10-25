using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal static class HandlerResultDescriptorBuilderExtensions
    {
        public static HandlerResultDescriptor ToDescriptor(this HandlerResult result)
        {
            string str = result.Data switch
            {
                null => string.Empty,
                string s => s,
                IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                _ => result.Data.ToString() ?? string.Empty
            };

            var data = AllocationUtilities.AllocString(str, out var dataLength);
            return new(result.StatusCode, data, dataLength, (int)result.ContentType);
        }
    }
}
