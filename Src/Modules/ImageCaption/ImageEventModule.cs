using AI.Common.Web;
using ImageCaption.Extensions;
using Microsoft.AspNetCore.Builder;

namespace ImageCaption;

public class ImageCaptionModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddImageModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseImageModules();
    }
}
