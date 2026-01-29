using AI.Common.Core;
using AI.Common.Jwt;
using AI.Common.MassTransit;
using AI.Common.OpenApi;
using AI.Common.PersistMessageProcessor;
using AI.Common.Web;
using Figgle;
using AI;
using Identity;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using ChatBot;
using ImageCaption;
using LearningAssistant;
using Meeting;
using Payment;
using Resume;
using User;

namespace Api.Extensions;

public static class SharedInfrastructureExtensions
{
    public static WebApplicationBuilder AddSharedInfrastructure(this WebApplicationBuilder builder)
    {
        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddServiceDefaults();

        builder.Services.AddJwt();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.AddPersistMessageProcessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.AddCustomMassTransit(
            builder.Environment,
            TransportType.InMemory,
            AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.Configure<ApiBehaviorOptions>(
            options => options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddGrpc(
            options =>
            {
                options.Interceptors.Add<GrpcExceptionInterceptor>();
            });

        builder.Services.AddEasyCaching(options => { options.UseInMemory(builder.Configuration, "mem"); });
        builder.Services.AddProblemDetails();

        builder.Services.AddScoped<IEventMapper>(sp =>
        {
            var mappers = new IEventMapper[] {
                                                 sp.GetRequiredService<ChatEventMapper>(),
                                                 sp.GetRequiredService<IdentityEventMapper>(),
                                                 sp.GetRequiredService<ResumeEventMapper>(),
                                                 sp.GetRequiredService<ImageEventMapper>(),
                                                 sp.GetRequiredService<AssistantEventMapper>(),
                                                 sp.GetRequiredService<MeetingEventMapper>(),
                                                 sp.GetRequiredService<PaymentEventMapper>(),
                                                 sp.GetRequiredService<UserEventMapper>(),
                                             };

            return new CompositeEventMapper(mappers);
        });


        return builder;
    }


    public static WebApplication UserSharedInfrastructure(this WebApplication app)
    {
        var appOptions = app.Configuration.GetOptions<AppOptions>(nameof(AppOptions));

        app.UseServiceDefaults();

        app.UseCustomProblemDetails();

        app.UseCorrelationId();

        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (app.Environment.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        return app;
    }
}