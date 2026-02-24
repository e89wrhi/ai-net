using AI.Common.Web;
using TextToSpeech.Extensions;
using Microsoft.AspNetCore.Builder;

namespace TextToSpeech;

public class TextToSpeechModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddTextToSpeechModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseTextToSpeechModules();
    }
}
