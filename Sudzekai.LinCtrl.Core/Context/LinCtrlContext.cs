using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Core 
{
    public static class LinCtrlContext
    {
        public const uint AbiVersion = 2;

        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private static readonly object _sync = new();

        public static void AddTypeInfoResolver(IJsonTypeInfoResolver resolver)
        {
            ArgumentNullException.ThrowIfNull(resolver);
            lock (_sync)
            {
                if (JsonSerializerOptions.TypeInfoResolverChain.Contains(resolver))
                    return;

                if (JsonSerializerOptions.IsReadOnly)
                    JsonSerializerOptions = new(JsonSerializerOptions);

                JsonSerializerOptions.TypeInfoResolverChain.Add(resolver);
            }
        }
    } 
}
