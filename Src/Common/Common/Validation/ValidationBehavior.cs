using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Common.Validation;

/// <summary>
/// A MediatR pipeline behavior that automatically triggers FluentValidation rules for every request.
/// If a validator exists for the request, it runs before the handler, 
/// ensuring that only valid data reaches our business logic.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private IValidator<TRequest> _validator;
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (_validator is null)
            return await next();

        await _validator.HandleValidationAsync(request);

        return await next();
    }
}