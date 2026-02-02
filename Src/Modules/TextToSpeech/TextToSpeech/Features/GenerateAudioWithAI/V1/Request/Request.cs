using TextToSpeech.Enums;

namespace TextToSpeech.Features.GenerateAudioWithAI.V1;

public record GenerateAudioWithAIRequestDto(string Text, VoiceType Voice);
public record GenerateAudioWithAIResponseDto(Guid SessionId, string AudioUrl);
