using AI.Common.Web;
using User.Extensions;
using Microsoft.AspNetCore.Builder;

namespace User;

public class UserModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddUserModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseUserModules();
    }
}
