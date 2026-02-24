using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Common.Web;

/// <summary>
/// Defines a contract for a module to register its services and middleware.
/// This allows for automated module discovery and registration.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Register module-specific services during the application build phase.
    /// </summary>
    WebApplicationBuilder AddModule(WebApplicationBuilder builder);

    /// <summary>
    /// Configure module-specific middleware during the application startup phase.
    /// </summary>
    WebApplication UseModule(WebApplication app);
}
