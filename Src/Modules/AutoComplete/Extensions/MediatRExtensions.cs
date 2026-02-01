using AI.Common.Caching;
using AI.Common.Logging;
using AI.Common.Validation;
using AutoComplete.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AutoComplete.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(AutoCompleteRoot).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EfTxAutocompleteBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));

        return services;
    }
}