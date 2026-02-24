using AI.Common.Web;
using ImageGen.Extensions;
using Microsoft.AspNetCore.Builder;

namespace ImageGen;

public class ImageGenModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddImageGenModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseImageGenModules();
    }
}
