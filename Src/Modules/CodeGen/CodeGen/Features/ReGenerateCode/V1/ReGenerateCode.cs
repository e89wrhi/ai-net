using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Exceptions;
using CodeGen.Models;
using CodeGen.ValueObjects;
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

namespace CodeGen.Features.SendCodeGen.V1;

public record SendCodeGenCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendCodeGenCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendCodeGenCommandResponse(Guid Id);

public record SendCodeGenRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendCodeGenRequestResponse(Guid Id);

public class SendCodeGenEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/send-message", async (SendCodeGenRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendCodeGenCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendCodeGenRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendCodeGen")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<SendCodeGenRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send CodeGen")
            .WithDescription("Send CodeGen")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendCodeGenCommandValidator : AbstractValidator<SendCodeGenCommand>
{
    public SendCodeGenCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendCodeGenHandler : IRequestHandler<SendCodeGenCommand, SendCodeGenCommandResponse>
{
    private readonly CodeGenDbContext _dbContext;

    public SendCodeGenHandler(CodeGenDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendCodeGenCommandResponse> Handle(SendCodeGenCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codegen = await _dbContext.CodeGens.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (codegen == null)
        {
            throw new CodeGenNotFoundException(request.SessionId);
        }

        var message = CodeGenModel.Create(
            CodeGenId.Of(NewId.NextGuid()),
            codegen.Id,
            ValueObjects.CodeGenerationPrompt.Of(request.Sender),
            IssueCount.Of(request.Content),
            ValueObjects.CodeGenerationConfiguration.Of(request.TokenUsed));

        codegen.AddCodeGen(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendCodeGenCommandResponse(message.Id.Value);
    }
}

