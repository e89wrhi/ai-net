using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record CodeGenerationStarted(Guid Id, string Prompt) : IIntegrationEvent;
public record CodeGenerated(Guid Id, string Language, int LinesCount) : IIntegrationEvent;
public record BoilerplateCreated(Guid Id, string Template) : IIntegrationEvent;
public record RefactoringSuggested(Guid Id, string Changes) : IIntegrationEvent;
