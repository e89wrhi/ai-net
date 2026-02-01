using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Sentiment.Models;

namespace Sentiment.Data;

public class SentimentReadDbContext : MongoDbContext
{
    public SentimentReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Sentiments = GetCollection<TextSentimentReadModel>(nameof(Sentiments).Underscore());
    }

    public IMongoCollection<TextSentimentReadModel> Sentiments { get; }
}