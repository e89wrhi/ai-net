using AI.Common.Web;
using SpeechToText.Extensions;
using Microsoft.AspNetCore.Builder;

namespace SpeechToText;

public class SpeechToTextModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddSpeechToTextModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseSpeechToTextModules();
    }
}
