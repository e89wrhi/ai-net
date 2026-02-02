using AI.Common.Core;

namespace User.Features.GenerateUserPersonaWithAI.V1;


public record GenerateUserPersonaWithAICommand(Guid UserId) : ICommand<GenerateUserPersonaWithAICommandResult>;

public record GenerateUserPersonaWithAICommandResult(string PersonaName, string Description, string Traits);
