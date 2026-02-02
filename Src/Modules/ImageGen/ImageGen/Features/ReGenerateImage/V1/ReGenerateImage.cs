using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Models;
using ImageGen.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.ReGenerateImage.V1;

public class ReGenerateImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/regenerate",
                async (ReGenerateImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new ReGenerateImageCommand(request.SessionId, request.Instruction);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ReGenerateImageResponseDto(result.ResultId, result.ImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReGenerateImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<ReGenerateImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Re-generate Image")
            .WithDescription("Re-generates or modifies an existing image based on new instructions.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
