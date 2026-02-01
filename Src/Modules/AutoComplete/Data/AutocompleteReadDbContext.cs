using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AutoComplete.Models;

namespace AutoComplete.Data;

public class AutocompleteReadDbContext : MongoDbContext
{
    public AutocompleteReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        AutoCompletes = GetCollection<AutoCompleteSessionReadModel>(nameof(AutoCompletes).Underscore());
    }

    public IMongoCollection<AutoCompleteSessionReadModel> AutoCompletes { get; }
}