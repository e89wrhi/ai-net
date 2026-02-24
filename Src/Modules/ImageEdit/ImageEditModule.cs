using AI.Common.Web;
using ImageEdit.Extensions;
using Microsoft.AspNetCore.Builder;

namespace ImageEdit;

public class ImageEditModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddImageEditModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseImageEditModules();
    }
}
