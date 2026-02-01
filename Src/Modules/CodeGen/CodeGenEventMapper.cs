using AI.Common.Contracts.EventBus.CodeGens;
using AI.Common.Core;
using CodeGen.Events;
using CodeGen.Features.DeleteCodeGen.V1;
using CodeGen.Features.SendCodeGen.V1;
using CodeGen.Features.StartCodeGen.V1;

namespace CodeGen;

public sealed class CodeGenEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            CodeGenerationSessionStartedDomainEvent e => new CodeGenSessionStarted(e.SessionId.Value),
            CodeGeneratedDomainEvent e => new CodeGenRecieved(e.CodeGenId.Value),
            CodeGenRespondedDomainEvent e => new CodeGenResponded(e.CodeGenId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            CodeGenerationSessionStartedDomainEvent e => new StartCodeGenMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            CodeGeneratedDomainEvent e => new SendCodeGenMongo(e.SessionId.Value, e.CodeGenId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            CodeGenRespondedDomainEvent e => new SendCodeGenMongo(e.SessionId.Value, e.CodeGenId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            CodeGenSessionDeletedDomainEvent e => new GenerateCodeMongo(e.Id.Value),
            _ => null
        };
    }
}