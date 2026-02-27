using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using AiOrchestration.Data;

namespace AiOrchestration.Services;

public interface IApiKeyService
{
    Task<UserApiKey> AddKeyAsync(Guid userId, string provider, string key, string label, CancellationToken ct = default);
    Task<IEnumerable<UserApiKey>> GetKeysAsync(Guid userId, CancellationToken ct = default);
    Task<UserApiKey?> GetKeyAsync(ApiKeyId id, CancellationToken ct = default);
    Task DeleteKeyAsync(ApiKeyId id, CancellationToken ct = default);
    Task<UserApiKey?> GetActiveKeyForProviderAsync(Guid userId, string provider, CancellationToken ct = default);
}

public class ApiKeyService : IApiKeyService
{
    private readonly AiOrchestrationDbContext _dbContext;

    public ApiKeyService(AiOrchestrationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserApiKey> AddKeyAsync(Guid userId, string provider, string key, string label, CancellationToken ct = default)
    {
        var apiKey = UserApiKey.Create(userId, provider, key, label);
        _dbContext.UserApiKeys.Add(apiKey);
        await _dbContext.SaveChangesAsync(ct);
        return apiKey;
    }

    public async Task<IEnumerable<UserApiKey>> GetKeysAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.UserApiKeys
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<UserApiKey?> GetKeyAsync(ApiKeyId id, CancellationToken ct = default)
    {
        return await _dbContext.UserApiKeys.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task DeleteKeyAsync(ApiKeyId id, CancellationToken ct = default)
    {
        var key = await _dbContext.UserApiKeys.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (key != null)
        {
            key.Deactivate();
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<UserApiKey?> GetActiveKeyForProviderAsync(Guid userId, string provider, CancellationToken ct = default)
    {
        return await _dbContext.UserApiKeys
            .Where(x => x.UserId == userId && x.ProviderName == provider && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }
}
