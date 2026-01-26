using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace AI.Common.OpenApi;

/// <summary>
/// A transformer that adds security definitions (like Bearer Token and API Key) 
/// to the generated OpenAPI document, allowing developers to test protected endpoints through the UI.
/// </summary>
public class SecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        document.Components ??= new();

        // Bearer token scheme
        document.Components.SecuritySchemes.Add(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "Enter 'Bearer' [space] and your token in the text input below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
            }
        );

        // API Key scheme
        document.Components.SecuritySchemes.Add(
            "ApiKey",
            new OpenApiSecurityScheme
            {
                Name = "X-API-KEY",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description =
                    "Enter your API key in the text input below.\n\nExample: '12345-abcdef'",
            }
        );

        return Task.CompletedTask;
    }
}