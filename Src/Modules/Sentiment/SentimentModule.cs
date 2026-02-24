using AI.Common.Web;
using Sentiment.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Sentiment;

public class SentimentModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddSentimentModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseSentimentModules();
    }
}
