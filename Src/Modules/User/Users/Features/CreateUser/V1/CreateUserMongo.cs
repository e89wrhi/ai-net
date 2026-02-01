using Ardalis.GuardClauses;
using User.Data;
using User.Models;
using MediatR;
using MongoDB.Driver;
using AI.Common.Core;

namespace User.Features.CreateUser.V1;

public record CreateUserMongo(Guid Id, string Username, string Email) : InternalCommand;

public class CreateUserMongoHandler : ICommandHandler<CreateUserMongo>
{
    private readonly UserReadDbContext _readDbContext;

    public CreateUserMongoHandler(UserReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(CreateUserMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = new UserAnalyticsReadModel
        {
            Id = request.Id,
            Username = request.Username,
            Email = request.Email,
            FullName = string.Empty,
            Activities = new List<UserActivityReadModel>(),
            Usages = new List<UsageContainerReadModel>()
        };

        await _readDbContext.User.InsertOneAsync(user, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
