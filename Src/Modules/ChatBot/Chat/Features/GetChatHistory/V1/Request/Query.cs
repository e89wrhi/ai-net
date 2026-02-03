using AI.Common.Caching;
using AI.Common.Core;
using ChatBot.Dtos;

namespace ChatBot.Features.GetChatHistory.V1;

public record GetChatHistory(Guid UserId) : IQuery<GetChatHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetChatHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetChatHistoryResult(IEnumerable<ChatDto> ChatDtos);

public record GetChatHistoryResponseDto(IEnumerable<ChatDto> ChatDtos);
