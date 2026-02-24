using System.Reflection;
using AI.Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Common.Web;

public static class ModuleExtensions
{
    private static readonly List<IModule> _discoveredModules = new();

    /// <summary>
    /// Automatically discovers and registers all classes implementing IModule in the solution.
    /// </summary>
    public static WebApplicationBuilder AddModules(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        var scanAssemblies = assemblies.Any()
            ? assemblies
            : TypeProvider.GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(TypeProvider.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        var moduleTypes = scanAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in moduleTypes)
        {
            var module = (IModule)Activator.CreateInstance(type)!;
            _discoveredModules.Add(module);
            module.AddModule(builder);
        }

        return builder;
    }

    /// <summary>
    /// Executes the middleware configuration for all discovered modules.
    /// </summary>
    public static WebApplication UseModules(this WebApplication app)
    {
        foreach (var module in _discoveredModules)
        {
            module.UseModule(app);
        }

        return app;
    }
}
