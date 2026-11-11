using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Patterns;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class PluginApiDescriptionProvider : IApiDescriptionProvider
    {
        private static readonly EmptyModelMetadataProvider _metadataProvider = new();

        public int Order => 0;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var description in context.Results)
            {
                var metadata = description.ActionDescriptor.EndpointMetadata.OfType<RequestHandlerMetadata>().LastOrDefault();
                if (metadata == null)
                    continue;

                if (metadata.AcceptsJson)
                {
                    AddParameter(description, "body", BindingSource.Body, typeof(JsonElement), true);
                    if (!description.SupportedRequestFormats.Any(format => format.MediaType == "application/json"))
                        description.SupportedRequestFormats.Add(new ApiRequestFormat { MediaType = "application/json" });
                }

                foreach (var parameter in RoutePatternFactory.Parse(metadata.Pattern).Parameters)
                {
                    var type = parameter.ParameterPolicies.FirstOrDefault()?.Content switch
                    {
                        "long" => typeof(long),
                        "int" => typeof(int),
                        _ => typeof(string)
                    };
                    AddParameter(description, parameter.Name, BindingSource.Path, type, !parameter.IsOptional);
                }

                foreach (var name in metadata.IntegerQueryParameters ?? [])
                    AddParameter(description, name, BindingSource.Query, typeof(int), false);
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }

        private static void AddParameter(ApiDescription description, string name, BindingSource source, Type type, bool required)
        {
            if (description.ParameterDescriptions.Any(parameter => parameter.Name == name && parameter.Source == source))
                return;

            description.ParameterDescriptions.Add(new ApiParameterDescription
            {
                Name = name,
                Source = source,
                Type = type,
                ModelMetadata = _metadataProvider.GetMetadataForType(type),
                IsRequired = required
            });
        }
    }
}
