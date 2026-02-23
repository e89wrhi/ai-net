using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record MarkdownProcessed(Guid Id, string Content) : IIntegrationEvent;
public record MarkdownMetadataExtracted(Guid Id, string Metadata) : IIntegrationEvent;
public record MarkdownConvertedToHtml(Guid Id) : IIntegrationEvent;
public record SharedLinkGenerated(Guid Id, string Link) : IIntegrationEvent;
