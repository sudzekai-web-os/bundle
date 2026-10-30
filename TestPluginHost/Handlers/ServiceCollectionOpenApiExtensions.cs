using Microsoft.OpenApi;

namespace Presentation
{
    internal static class ServiceCollectionOpenApiExtensions
    {
        public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
            => services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((doc, ctx, ct) =>
                {
                    doc.Components ??= new OpenApiComponents();
                    doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };

                    return Task.CompletedTask;
                });
            });
    }
}
