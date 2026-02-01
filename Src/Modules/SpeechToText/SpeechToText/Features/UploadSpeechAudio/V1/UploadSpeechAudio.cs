using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Exceptions;
using SpeechToText.ValueObjects;
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

namespace SpeechToText.Features.DeleteSpeechToText.V1;

public record DeleteSpeechToTextCommand(Guid SessionId) : ICommand<DeleteSpeechToTextCommandResponse>;

public record DeleteSpeechToTextCommandResponse(Guid Id);

public record DeleteSpeechToTextRequest(Guid SessionId);

public record DeleteSpeechToTextRequestResponse(Guid Id);

public class DeleteSpeechToTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/speechtotext/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteSpeechToTextCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteSpeechToTextRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteSpeechToText")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<DeleteSpeechToTextRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete SpeechToText")
            .WithDescription("Delete SpeechToText")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteSpeechToTextCommandValidator : AbstractValidator<DeleteSpeechToTextCommand>
{
    public DeleteSpeechToTextCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteSpeechToTextHandler : IRequestHandler<DeleteSpeechToTextCommand, DeleteSpeechToTextCommandResponse>
{
    private readonly SpeechToTextDbContext _dbContext;

    public DeleteSpeechToTextHandler(SpeechToTextDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteSpeechToTextCommandResponse> Handle(DeleteSpeechToTextCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var speechtotext = await _dbContext.SpeechToTexts.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (speechtotext == null)
        {
            throw new SpeechToTextNotFoundException(request.SessionId);
        }

        speechtotext.Delete();
        _dbContext.SpeechToTexts.Remove(speechtotext);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteSpeechToTextCommandResponse(speechtotext.Id.Value);
    }
}

