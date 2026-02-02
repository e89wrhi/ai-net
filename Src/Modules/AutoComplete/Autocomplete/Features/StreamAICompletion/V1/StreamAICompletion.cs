using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.StreamAICompletion.V1;

public class StreamAICompletionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/stream",
                (StreamAICompletionRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    // Mock User ID or extract from context
                    var userId = Guid.NewGuid();
                    var command = new StreamAutoCompleteCommand(userId, request.Prompt);
                    
                    return mediator.CreateStream(command, cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAICompletion")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream AI Completion")
            .WithDescription("Streams text completion using an AI model.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
