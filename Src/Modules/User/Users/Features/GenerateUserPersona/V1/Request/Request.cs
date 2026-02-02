namespace User.Features.GenerateUserPersona.V1;

public record GenerateUserPersonaWithAIRequestDto(Guid UserId);
public record GenerateUserPersonaWithAIResponseDto(string PersonaName, string Description, string Traits);
