using AI.Common.Core;

namespace ChatBot.Features.GetChatById.V1;

public record GetChatByIdQuery(Guid SessionId, Guid UserId) : IQuery<GetChatByIdResult>;
