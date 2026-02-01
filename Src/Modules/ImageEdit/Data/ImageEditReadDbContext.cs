using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ImageEdit.Models;

namespace ImageEdit.Data;

public class ImageEditReadDbContext : MongoDbContext
{
    public ImageEditReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        ImageEdits = GetCollection<ImageEditSessionReadModel>(nameof(ImageEdits).Underscore());
    }

    public IMongoCollection<ImageEditSessionReadModel> ImageEdits { get; }
}