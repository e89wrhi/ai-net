using Asp.Versioning;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Common.Web;

/// <summary>
/// The parent of all API controllers in our system.
/// It provides a common routing pattern (api/v1/...) and quick access to 
/// essential tools like Mediator and Mapper without having to inject them manually every time.
/// </summary>
[Route(BaseApiPath)]
[ApiController]
[ApiVersion("1.0")]
public abstract class BaseController : ControllerBase
{
    protected const string BaseApiPath = "api/v{version:apiVersion}";
    private IMapper _mapper;

    private IMediator _mediator;

    /// <summary>
    /// For sending commands and queries.
    /// It's Resolve-on-demand to keep the constructor of your child controllers clean.
    /// </summary>
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    /// <summary>
    /// For mapping between Domain objects and DTOs.
    /// </summary>
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
}