using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Translate.Models;

namespace Translate.Data;

public class TranslateReadDbContext : MongoDbContext
{
    public TranslateReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Translates = GetCollection<TranslationSessionReadModel>(nameof(Translates).Underscore());
    }

    public IMongoCollection<TranslationSessionReadModel> Translates { get; }
}