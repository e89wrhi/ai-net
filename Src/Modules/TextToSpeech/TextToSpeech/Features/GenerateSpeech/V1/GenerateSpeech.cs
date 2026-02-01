using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Exceptions;
using TextToSpeech.ValueObjects;
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

namespace TextToSpeech.Features.DeleteTextToSpeech.V1;

public record DeleteTextToSpeechCommand(Guid SessionId) : ICommand<DeleteTextToSpeechCommandResponse>;

public record DeleteTextToSpeechCommandResponse(Guid Id);

public record DeleteTextToSpeechRequest(Guid SessionId);

public record DeleteTextToSpeechRequestResponse(Guid Id);

public class DeleteTextToSpeechEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/texttospeech/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteTextToSpeechCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteTextToSpeechRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteTextToSpeech")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<DeleteTextToSpeechRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete TextToSpeech")
            .WithDescription("Delete TextToSpeech")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteTextToSpeechCommandValidator : AbstractValidator<DeleteTextToSpeechCommand>
{
    public DeleteTextToSpeechCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteTextToSpeechHandler : IRequestHandler<DeleteTextToSpeechCommand, DeleteTextToSpeechCommandResponse>
{
    private readonly TextToSpeechDbContext _dbContext;

    public DeleteTextToSpeechHandler(TextToSpeechDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteTextToSpeechCommandResponse> Handle(DeleteTextToSpeechCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var texttospeech = await _dbContext.TextToSpeechs.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (texttospeech == null)
        {
            throw new TextToSpeechNotFoundException(request.SessionId);
        }

        texttospeech.Delete();
        _dbContext.TextToSpeechs.Remove(texttospeech);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteTextToSpeechCommandResponse(texttospeech.Id.Value);
    }
}

