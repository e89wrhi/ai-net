using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record PluginLoaded(Guid Id, string PluginName) : IIntegrationEvent;
public record PluginExecuted(Guid Id, string PluginName, bool Success) : IIntegrationEvent;
public record PluginErrorOccurred(Guid Id, string PluginName, string ErrorMessage) : IIntegrationEvent;
public record ToolRegistered(Guid Id, string ToolName) : IIntegrationEvent;
