namespace AiOrchestration.Dtos;

public record ApiKeyDto(Guid Id, string ProviderName, string Label, bool IsActive, DateTime? LastUsedAt);
