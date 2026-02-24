using AI.Common.Web;
using SimplePlugin.Extensions;
using Microsoft.AspNetCore.Builder;

namespace SimplePlugin;

public class SimplePluginModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddSimplePluginModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseSimplePluginModules();
    }
}
