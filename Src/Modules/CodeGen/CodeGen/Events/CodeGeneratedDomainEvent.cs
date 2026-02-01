using AI.Common.Core;
using CodeGen.ValueObjects;

namespace CodeGen.Events;

public record CodeGeneratedDomainEvent(CodeGenId Id, CodeGenResultId ResultId, string Language) : IDomainEvent;
