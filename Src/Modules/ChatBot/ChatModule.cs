using AI.Common.Web;
using ChatBot.Extensions;
using Microsoft.AspNetCore.Builder;

namespace ChatBot;

public class ChatModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddChatModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseChatModules();
    }
}
