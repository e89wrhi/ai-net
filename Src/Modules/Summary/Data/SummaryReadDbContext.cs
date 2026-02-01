using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Summary.Models;

namespace Summary.Data;

public class SummaryReadDbContext : MongoDbContext
{
    public SummaryReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Summaries = GetCollection<TextSummaryReadModel>(nameof(Summaries).Underscore());
    }

    public IMongoCollection<TextSummaryReadModel> Summaries { get; }
}