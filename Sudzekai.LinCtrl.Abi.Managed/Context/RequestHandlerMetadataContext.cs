using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    [JsonSerializable(typeof(RequestHandlerMetadata[]))]
    public partial class RequestHandlerMetadataContext : JsonSerializerContext
    {
    }
}
