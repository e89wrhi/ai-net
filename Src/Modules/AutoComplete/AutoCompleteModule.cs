using AI.Common.Web;
using AutoComplete.Extensions;
using Microsoft.AspNetCore.Builder;

namespace AutoComplete;

public class AutoCompleteModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddAutoCompleteModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseAutoCompleteModules();
    }
}
