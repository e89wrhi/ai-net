using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;
using TextToSpeech.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace TextToSpeech.Features.GenerateAudioWithAI.V1;

public record GenerateAudioWithAICommand(string Text, VoiceType Voice) : ICommand<GenerateAudioWithAIResult>;

public record GenerateAudioWithAIResult(Guid SessionId, string AudioUrl);

public class GenerateAudioWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/generate-ai",
                async (GenerateAudioWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAudioWithAICommand(request.Text, request.Voice);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAudioWithAIResponseDto(result.SessionId, result.AudioUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAudioWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<GenerateAudioWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Voice")
            .WithDescription("Uses AI to generate expressive spoken audio from text with selection of voice types.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record GenerateAudioWithAIRequestDto(string Text, VoiceType Voice);
public record GenerateAudioWithAIResponseDto(Guid SessionId, string AudioUrl);

internal class GenerateAudioWithAIHandler : ICommandHandler<GenerateAudioWithAICommand, GenerateAudioWithAIResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public GenerateAudioWithAIHandler(TextToSpeechDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAudioWithAIResult> Handle(GenerateAudioWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        // Mock Logic for AI Voice Generation
        var audioUrl = $"https://cdn.ai-voices.com/gen/{Guid.NewGuid()}.wav";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "tts-expressive-1");
        var config = new TextToSpeechConfiguration(request.Voice, SpeechSpeed.Normal, LanguageCode.Of("en"));

        var session = TextToSpeechSession.Create(sessionId, userId, modelId, config);

        var resultId = TextToSpeechResultId.Of(Guid.NewGuid());
        var speechVo = SynthesizedSpeech.Of(audioUrl);
        var tokenCountVo = TokenCount.Of(request.Text.Length);
        var costVo = CostEstimate.Of(0.02m);

        var result = TextToSpeechResult.Create(resultId, request.Text, speechVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAudioWithAIResult(sessionId.Value, audioUrl);
    }
}
