using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Resume.Models;

namespace Resume.Data;

public class ResumeReadDbContext : MongoDbContext
{
    public ResumeReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Resume = GetCollection<ResumeReadModel>(nameof(Resume).Underscore());
    }

    public IMongoCollection<ResumeReadModel> Resume { get; }
}