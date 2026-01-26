using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AI.Common.Mapster;

/// <summary>
/// Infrastructure extensions for setting up Mapster.
/// It automatically scans the provided assemblies for mapping configurations,
/// making it easy to convert between Domain models and DTOs.
/// </summary>
public static class Extensions
{
    public static IServiceCollection AddCustomMapster(this IServiceCollection services, params Assembly[] assemblies)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(assemblies);
        var mapperConfig = new Mapper(typeAdapterConfig);
        services.AddSingleton<IMapper>(mapperConfig);

        return services;
    }
}