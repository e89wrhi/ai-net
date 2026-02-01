using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ImageGen.Models;

namespace ImageGen.Data;

public class ImageGenReadDbContext : MongoDbContext
{
    public ImageGenReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        ImageGenerations = GetCollection<ImageGenerationReadModel>(nameof(ImageGenerations).Underscore());
    }

    public IMongoCollection<ImageGenerationReadModel> ImageGenerations { get; }
}