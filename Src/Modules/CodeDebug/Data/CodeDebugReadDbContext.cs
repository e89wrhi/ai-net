using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CodeDebug.Models;

namespace CodeDebug.Data;

public class CodeDebugReadDbContext : MongoDbContext
{
    public CodeDebugReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        CodeDebugs = GetCollection<CodeDebugSessionReadModel>(nameof(CodeDebugs).Underscore());
    }

    public IMongoCollection<CodeDebugSessionReadModel> CodeDebugs { get; }
}