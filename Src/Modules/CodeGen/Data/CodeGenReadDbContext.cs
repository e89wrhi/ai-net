using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CodeGen.Models;

namespace CodeGen.Data;

public class CodeGenReadDbContext : MongoDbContext
{
    public CodeGenReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        CodeGenerations = GetCollection<CodeGenerationReadModel>(nameof(CodeGenerations).Underscore());
    }

    public IMongoCollection<CodeGenerationReadModel> CodeGenerations { get; }
}