using AI.Common.Web;
using Resume.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Resume;

public class ResumeModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddResumeModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseResumeModules();
    }
}
