using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
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

namespace ChatBot.Features.SendMessage.V1;

public class SendMessageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat/send-message", async (SendMessageRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendMessageCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendMessageRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendMessage")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<SendMessageRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send Message")
            .WithDescription("Send Message")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
