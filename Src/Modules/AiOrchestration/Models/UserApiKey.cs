using AI.Common.Core.Model;
using AiOrchestration.ValueObjects;

namespace AiOrchestration.Models;

public record UserApiKey : Aggregate<ApiKeyId>
{
    public Guid UserId { get; private set; }
    public string ProviderName { get; private set; } = default!;
    public string ApiKey { get; private set; } = default!;
    public string Label { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime? LastUsedAt { get; private set; }

    private UserApiKey() { }

    public static UserApiKey Create(Guid userId, string providerName, string apiKey, string label)
    {
        return new UserApiKey
        {
            Id = ApiKeyId.Of(Guid.NewGuid()),
            UserId = userId,
            ProviderName = providerName,
            ApiKey = apiKey,
            Label = label,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void MarkUsed() => LastUsedAt = DateTime.UtcNow;
}
