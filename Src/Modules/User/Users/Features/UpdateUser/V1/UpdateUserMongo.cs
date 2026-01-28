using Ardalis.GuardClauses;
using User.Data;
using User.Models;
using MediatR;
using MongoDB.Driver;
using AI.Common.Core;

namespace User.Features.UpdateUser.V1;

public record UpdateUserMongo(Guid Id, string FullName) : InternalCommand;

public class UpdateUserMongoHandler : ICommandHandler<UpdateUserMongo>
{
    private readonly UserReadDbContext _readDbContext;

    public UpdateUserMongoHandler(UserReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UpdateUserMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<UserReadModel>.Filter.Eq(x => x.Id, request.Id);
        var update = Builders<UserReadModel>.Update.Set(x => x.FullName, request.FullName);

        await _readDbContext.User.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
