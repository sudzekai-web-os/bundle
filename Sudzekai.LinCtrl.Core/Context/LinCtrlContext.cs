using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Core 
{
    public static class LinCtrlContext
    {
        public const uint AbiVersion = 1;

        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static void AddTypeInfoResolver(IJsonTypeInfoResolver resolver)
            => JsonSerializerOptions.TypeInfoResolverChain.Add(resolver);
    } 
}
