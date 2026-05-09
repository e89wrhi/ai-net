
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace AI.Common.OpenApi;

/// <summary>
/// Infrastructure extensions for setting up OpenAPI (Swagger) documentation.
/// It configures versioned API endpoints and integrates with Scalar for a premium API documentation experience.
/// </summary>
public static class Extensions
{
    // ref: https://github.com/dotnet/eShop/blob/main/src/eShop.ServiceDefaults/OpenApi.Extensions.cs
    public static IServiceCollection AddAspnetOpenApi(this IServiceCollection services)
    {
        string[] versions = ["v1"];

        foreach (var description in versions)
        {
            services.AddOpenApi(
                description,
                options =>
                {
                    options.AddDocumentTransformer<SecuritySchemeDocumentTransformer>();
                    options.AddOperationTransformer<AuthOperationTransformer>();
                });
        }

        return services;
    }

    public static IApplicationBuilder UseAspnetOpenApi(this WebApplication app)
    {
        app.MapOpenApi();

        // Add scalar ui
        app.MapScalarApiReference();

        return app;
    }
}