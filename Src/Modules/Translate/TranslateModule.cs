using AI.Common.Web;
using Translate.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Translate;

public class TranslateModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddTranslateModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseTranslateModules();
    }
}
