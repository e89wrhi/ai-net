using AI.Common.Core;
using MediatR;
using User.Data;

namespace User.Features.UpdateUser.V1;

internal class UpdateUserHandler : ICommandHandler<UpdateUserCommand, UpdateUserCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public UpdateUserHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Minimal implementation: In this module, "UpdateUser" might just be a notification 
        // or we might want to update some user-specific analytics settings in the future.
        // For now, we just return success to fix the build and provide a working skeleton.
        
        return new UpdateUserCommandResponse(true);
    }
}
