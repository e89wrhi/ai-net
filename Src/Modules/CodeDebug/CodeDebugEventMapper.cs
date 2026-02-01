using AI.Common.Contracts.EventBus.CodeDebugs;
using AI.Common.Core;
using CodeDebug.Events;
using CodeDebug.Features.DeleteCodeDebug.V1;
using CodeDebug.Features.SendCodeDebug.V1;
using CodeDebug.Features.StartCodeDebug.V1;

namespace CodeDebug;

public sealed class CodeDebugEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            CodeDebugSessionStartedDomainEvent e => new CodeDebugSessionStarted(e.SessionId.Value),
            CodeDebugAnalyzedDomainEvent e => new CodeDebugRecieved(e.CodeDebugId.Value),
            CodeDebugRespondedDomainEvent e => new CodeDebugResponded(e.CodeDebugId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            CodeDebugSessionStartedDomainEvent e => new StartCodeDebugMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            CodeDebugAnalyzedDomainEvent e => new SendCodeDebugMongo(e.SessionId.Value, e.CodeDebugId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            CodeDebugRespondedDomainEvent e => new SendCodeDebugMongo(e.SessionId.Value, e.CodeDebugId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            CodeDebugSessionDeletedDomainEvent e => new DebugCodeMongo(e.Id.Value),
            _ => null
        };
    }
}