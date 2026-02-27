using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using AiOrchestration.Data;

namespace AiOrchestration.Services;

public interface IUsageService
{
    Task LogUsageAsync(Guid userId, ModelId modelId, int tokens, decimal cost, string? provider = null, Guid? apiKeyId = null, CancellationToken ct = default);
    Task<IEnumerable<AiUsage>> GetUsageAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
}

public class UsageService : IUsageService
{
    private readonly AiOrchestrationDbContext _dbContext;

    public UsageService(AiOrchestrationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogUsageAsync(Guid userId, ModelId modelId, int tokens, decimal cost, string? provider = null, Guid? apiKeyId = null, CancellationToken ct = default)
    {
        var usage = AiUsage.Create(userId, modelId, tokens, cost, provider, apiKeyId);
        _dbContext.UsageLogs.Add(usage);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AiUsage>> GetUsageAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var query = _dbContext.UsageLogs.Where(x => x.UserId == userId);
        if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(x => x.CreatedAt <= to.Value);
        
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }
}
