using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Sentiment.Data;
using Sentiment.Exceptions;
using Sentiment.ValueObjects;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MassTransit.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sentiment.Features.DeleteSentiment.V1;

public record DeleteSentimentCommand(Guid SessionId) : ICommand<DeleteSentimentCommandResponse>;

public record DeleteSentimentCommandResponse(Guid Id);

public record DeleteSentimentRequest(Guid SessionId);

public record DeleteSentimentRequestResponse(Guid Id);

public class DeleteSentimentEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/sentiment/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteSentimentCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteSentimentRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteSentiment")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<DeleteSentimentRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Sentiment")
            .WithDescription("Delete Sentiment")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteSentimentCommandValidator : AbstractValidator<DeleteSentimentCommand>
{
    public DeleteSentimentCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteSentimentHandler : IRequestHandler<DeleteSentimentCommand, DeleteSentimentCommandResponse>
{
    private readonly SentimentDbContext _dbContext;

    public DeleteSentimentHandler(SentimentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteSentimentCommandResponse> Handle(DeleteSentimentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var sentiment = await _dbContext.Sentiments.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (sentiment == null)
        {
            throw new SentimentNotFoundException(request.SessionId);
        }

        sentiment.Delete();
        _dbContext.Sentiments.Remove(sentiment);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteSentimentCommandResponse(sentiment.Id.Value);
    }
}

