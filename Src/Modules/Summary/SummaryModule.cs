using AI.Common.Web;
using Summary.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Summary;

public class SummaryModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddSummaryModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseSummaryModules();
    }
}
