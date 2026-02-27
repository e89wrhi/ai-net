using AiOrchestration.Data;
using AiOrchestration.Services;
using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.AspNetCore.Builder;

namespace AiOrchestration;

public class AiOrchestrationModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAiModelService, AiModelService>();
        builder.Services.AddScoped<IAiOrchestrator, AiOrchestrator>();
        builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
        builder.Services.AddScoped<IUsageService, UsageService>();
        builder.AddCustomDbContext<AiOrchestrationDbContext>(nameof(AiOrchestration));

        builder.AddMinimalEndpoints(assemblies: typeof(AiOrchestrationRoot).Assembly);
        
        return builder;
    }

    public WebApplication UseModule(WebApplication app)
    {
        app.UseMigration<AiOrchestrationDbContext>();
        
        return app;
    }
}

