using AI.Common.Core;
using CodeGen.ValueObjects;

namespace CodeGen.Events;

public record CodeGenerationSessionCompletedDomainEvent(CodeGenId Id): IDomainEvent;
