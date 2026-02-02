using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using CodeGen.Data;
using CodeGen.Data.Seed;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGen.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddCodeGenModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<CodeGenEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(CodeGenRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(CodeGenRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(CodeGenRoot).Assembly);
        builder.AddCustomDbContext<CodeGenDbContext>(nameof(CodeGen));
        builder.Services.AddScoped<IDataSeeder, CodeGenDataSeeder>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseCodeGenModules(this WebApplication app)
    { 
        app.UseMigration<CodeGenDbContext>();
        app.MapGrpcService<CodeGenGrpcService>();

        return app;
    }
}