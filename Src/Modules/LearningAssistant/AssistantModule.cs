using AI.Common.Web;
using LearningAssistant.Extensions;
using Microsoft.AspNetCore.Builder;

namespace LearningAssistant;

public class LearningAssistantModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddAssistantModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseAssistantModules();
    }
}
