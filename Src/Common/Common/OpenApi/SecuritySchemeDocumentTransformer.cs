using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AI.Common.OpenApi;

public sealed class SecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
       OpenApiDocument document,
       OpenApiDocumentTransformerContext context,
       CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "JWT Bearer token"
            };

        return Task.CompletedTask;
    }
}