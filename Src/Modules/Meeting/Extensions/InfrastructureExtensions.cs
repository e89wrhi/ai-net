using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using Meeting;
using Meeting.Data;
using Meeting.Data.Seed;
using Meeting.Extensions;
using Meeting.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Meeting.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddMeetingModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<MeetingEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(MeetingRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(MeetingRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(MeetingRoot).Assembly);
        builder.AddCustomDbContext<MeetingDbContext>(nameof(Meeting));
        builder.Services.AddScoped<IDataSeeder, MeetingDataSeeder>();
        builder.AddMongoDbContext<MeetingReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseMeetingModules(this WebApplication app)
    { 
        app.UseMigration<MeetingDbContext>();
        app.MapGrpcService<MeetingGrpcServices>();

        return app;
    }
}