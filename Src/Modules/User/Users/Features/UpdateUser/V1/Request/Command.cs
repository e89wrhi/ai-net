using AI.Common.Core;

namespace User.Features.UpdateUser.V1;

public record UpdateUserCommand(Guid SessionId, string? FullName, string? Email) : ICommand<UpdateUserCommandResponse>;

public record UpdateUserCommandResponse(bool Success);
