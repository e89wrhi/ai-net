using AI.Common.Web;
using Identity.Extensions.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace Identity;

public class IdentityModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddIdentityModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseIdentityModules();
    }
}
