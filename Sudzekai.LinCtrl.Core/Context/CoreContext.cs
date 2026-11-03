using System.Text.Json;

namespace Sudzekai.LinCtrl.Core 
{
    public static class CoreContext
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
