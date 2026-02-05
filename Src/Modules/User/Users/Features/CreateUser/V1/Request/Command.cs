using AI.Common.Core;

namespace User.Features.CreateUser.V1;

public record CreateUserCommand(Guid UserId) : ICommand<CreateUserCommandResponse>;

public record CreateUserCommandResponse(Guid Id);
