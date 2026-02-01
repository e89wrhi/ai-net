using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TextToSpeech.Models;

namespace TextToSpeech.Data;

public class TextToSpeechReadDbContext : MongoDbContext
{
    public TextToSpeechReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        TextToSpeechs = GetCollection<TextToSpeechSessionReadModel>(nameof(TextToSpeechs).Underscore());
    }

    public IMongoCollection<TextToSpeechSessionReadModel> TextToSpeechs { get; }
}