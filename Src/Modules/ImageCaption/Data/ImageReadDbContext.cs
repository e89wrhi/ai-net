using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ImageCaption.Models;

namespace ImageCaption.Data;

public class ImageReadDbContext : MongoDbContext
{
    public ImageReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Image = GetCollection<ImageReadModel>(nameof(Image).Underscore());
    }

    public IMongoCollection<ImageReadModel> Image { get; }
}