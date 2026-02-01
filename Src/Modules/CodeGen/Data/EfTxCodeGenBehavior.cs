using AI.Common.Core;
using AI.Common.PersistMessageProcessor;
using AI.Common.Polly;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Transactions;

namespace CodeGen.Data;

public class EfTxCodeGenBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull, IRequest<TResponse>
where TResponse : notnull
{
    private readonly ILogger<EfTxCodeGenBehavior<TRequest, TResponse>> _logger;
    private readonly CodeGenDbContext _dbContext;
    private readonly IPersistMessageDbContext _persistCodeGenDbContext;
    private readonly IEventDispatcher _eventDispatcher;

    public EfTxCodeGenBehavior(
        ILogger<EfTxCodeGenBehavior<TRequest, TResponse>> logger,
        CodeGenDbContext dbContext,
        IPersistMessageDbContext persistCodeGenDbContext,
        IEventDispatcher eventDispatcher
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _persistCodeGenDbContext = persistCodeGenDbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            GetType().Name,
            typeof(TRequest).FullName);

        _logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            GetType().Name,
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        var response = await next();

        _logger.LogInformation(
            "{Prefix} Executed the {MediatrRequest} request",
            GetType().Name,
            typeof(TRequest).FullName);

        while (true)
        {
            var domainEvents = _dbContext.GetDomainEvents();

            if (domainEvents is null || !domainEvents.Any())
            {
                return response;
            }

            _logger.LogInformation(
                "{Prefix} Open the transaction for {MediatrRequest}",
                GetType().Name,
                typeof(TRequest).FullName);

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);

            await _eventDispatcher.SendAsync(
                domainEvents.ToArray(),
                typeof(TRequest),
                cancellationToken);

            // Save data to database with some retry policy in distributed transaction
            await _dbContext.RetryOnFailure(
                async () =>
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                });

            // Save data to database with some retry policy in distributed transaction
            await _persistCodeGenDbContext.RetryOnFailure(
                async () =>
                {
                    await _persistCodeGenDbContext.SaveChangesAsync(cancellationToken);
                });

            scope.Complete();

            return response;
        }
    }
}