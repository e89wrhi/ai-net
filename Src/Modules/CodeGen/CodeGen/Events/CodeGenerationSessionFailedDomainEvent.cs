using AI.Common.Core;
using CodeGen.Enums;
using CodeGen.ValueObjects;

namespace CodeGen.Events;

public record CodeGenerationSessionFailedDomainEvent(CodeGenId Id, CodeGenerationFailureReason Reason): IDomainEvent;
