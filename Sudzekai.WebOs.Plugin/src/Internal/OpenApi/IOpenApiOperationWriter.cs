using Sudzekai.WebOs.Core;
using System.Text.Json;

namespace Sudzekai.WebOs.Plugin
{
    internal interface IOpenApiOperationWriter
    {
        void WriteOperation(Utf8JsonWriter writer, IEndpointHandler handler);
    }
}