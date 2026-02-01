using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeGen.ValueObjects;

namespace CodeGen.Events;

public record CodeGenerationSessionStartedDomainEvent(CodeGenId Id, UserId UserId, ModelId AiModel) : IDomainEvent;
