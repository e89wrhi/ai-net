using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using User.Models;

namespace User.Data;

public class UserReadDbContext : MongoDbContext
{
    public UserReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        User = GetCollection<UserAnalyticsReadModel>(nameof(User).Underscore());
    }

    public IMongoCollection<UserAnalyticsReadModel> User { get; }
}