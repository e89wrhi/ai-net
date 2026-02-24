using AI.Common.Web;
using CodeGen.Extensions;
using Microsoft.AspNetCore.Builder;

namespace CodeGen;

public class CodeGenModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddCodeGenModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UseCodeGenModules();
    }
}
