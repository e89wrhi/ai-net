using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LearningAssistant.Models;

namespace LearningAssistant.Data;

public class LearningReadDbContext : MongoDbContext
{
    public LearningReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Profiles = GetCollection<LearningSessionReadModel>(nameof(Profiles).Underscore());
    }

    public IMongoCollection<LearningSessionReadModel> Profiles { get; }
}