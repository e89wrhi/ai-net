using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using SimpleMD.Services;

namespace SimpleMD.Extensions;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSimpleMDModules(this WebApplicationBuilder builder)
    {
        builder.AddMinimalEndpoints(assemblies: typeof(SimpleMDRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SimpleMDRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SimpleMDRoot).Assembly);

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Bind markdown file options from "SimpleMD" section in appsettings.
        // Override SimpleMD:FilePath in appsettings to point to a custom .md file.
        builder.Services.Configure<MarkdownFileOptions>(
            builder.Configuration.GetSection(MarkdownFileOptions.SectionName));

        // Register the markdown file provider (file I/O abstraction used by all handlers).
        builder.Services.AddSingleton<IMarkdownFileProvider, MarkdownFileProvider>();

        // Register AI Chat Client.
        // In a real scenario, replace SimulatedChatClient with a real provider (e.g. OpenAI, Ollama).
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }

    public static WebApplication UseSimpleMDModules(this WebApplication app)
    {
        return app;
    }
}