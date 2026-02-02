using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CodeDebug.Features.AnalyzeCode.V1;

public class AnalyzeCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codedebug/analyze",
                async (AnalyzeCodeRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeCodeCommand(request.Code, request.Language);
                    var result = await mediator.Send(command, cancellationToken);

                    return Results.Ok(new AnalyzeCodeResponseDto(result.SessionId, result.ReportId, result.Summary, result.IssueCount));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<AnalyzeCodeResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Code")
            .WithDescription("Analyzes code for bugs and issues using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
