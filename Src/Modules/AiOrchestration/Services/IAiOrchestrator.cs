using Microsoft.Extensions.AI;
using AiOrchestration.Models;

namespace AiOrchestration.Services;

public interface IAiOrchestrator
{
    ChatClientMetadata Metadata { get; }
    Task<IChatClient> GetClientAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default);
    Task<AiModel> SelectModelAsync(ModelCriteria? criteria = null, CancellationToken cancellationToken = default);
}
