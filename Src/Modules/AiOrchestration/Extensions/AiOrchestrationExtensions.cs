using AiOrchestration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiOrchestration.Extensions;

public static class AiOrchestrationExtensions
{
    public static IServiceCollection AddAiOrchestration(this IServiceCollection services)
    {
        services.AddSingleton<IAiModelService, AiModelService>();
        services.AddScoped<IAiOrchestrator, AiOrchestrator>();
        return services;
    }
}
