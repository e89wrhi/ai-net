using AI.Common.Web;
using Meeting.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Meeting;

public class MeetingModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddMeetingModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseMeetingModules();
    }
}
