using AI.Common.Web;
using CodeDebug.Extensions;
using Microsoft.AspNetCore.Builder;

namespace CodeDebug;

public class CodeDebugModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddCodeDebugModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseCodeDebugModules();
    }
}
