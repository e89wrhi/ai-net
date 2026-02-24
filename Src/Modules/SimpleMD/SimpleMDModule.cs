using AI.Common.Web;
using SimpleMD.Extensions;
using Microsoft.AspNetCore.Builder;

namespace SimpleMD;

public class SimpleMDModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddSimpleMDModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseSimpleMDModules();
    }
}
