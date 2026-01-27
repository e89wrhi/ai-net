using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LearningAssistant.Models;

namespace LearningAssistant.Data;

public class ProfileReadDbContext : MongoDbContext
{
    public ProfileReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Profiles = GetCollection<ProfileReadModel>(nameof(Profiles).Underscore());
    }

    public IMongoCollection<ProfileReadModel> Profiles { get; }
}