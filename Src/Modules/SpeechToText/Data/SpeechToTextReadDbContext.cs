using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpeechToText.Models;

namespace SpeechToText.Data;

public class SpeechToTextReadDbContext : MongoDbContext
{
    public SpeechToTextReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        SpeechToTexts = GetCollection<SpeechToTextSessionReadModel>(nameof(SpeechToTexts).Underscore());
    }

    public IMongoCollection<SpeechToTextSessionReadModel> SpeechToTexts { get; }
}