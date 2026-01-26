using Microsoft.AspNetCore.Routing;

namespace AI.Common.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}