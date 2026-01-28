using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.ValueObjects;
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

namespace LearningAssistant.Features.GenerateQuize.V1;


public record GenerateQuizeCommand(Guid LessonId, string Questions) : ICommand<GenerateQuizeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateQuizeCommandResponse(Guid Id);

public record GenerateQuizeRequest(Guid LessonId, string Questions);

public record GenerateQuizeRequestResponse(Guid Id);

public class GenerateQuizeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/quize", async (GenerateQuizeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateQuizeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateQuizeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateQuize")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateQuizeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Quize")
            .WithDescription("Generate Quize")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GenerateQuizeCommandValidator : AbstractValidator<GenerateQuizeCommand>
{
    public GenerateQuizeCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.Questions).NotEmpty();
    }
}

internal class GenerateQuizeHandler : IRequestHandler<GenerateQuizeCommand, GenerateQuizeCommandResponse>
{
    private readonly AssistantDbContext _dbContext;

    public GenerateQuizeHandler(AssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateQuizeCommandResponse> Handle(GenerateQuizeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var lesson = await _dbContext.Lessons.FindAsync(new object[] { LessonId.Of(request.LessonId) }, cancellationToken);

        if (lesson == null)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        var quize = QuizeModel.Create(
            QuizeId.Of(NewId.NextGuid()),
            lesson.Id,
            request.Questions);

        lesson.AddQuize(quize);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new GenerateQuizeCommandResponse(quize.Id.Value);
    }
}

