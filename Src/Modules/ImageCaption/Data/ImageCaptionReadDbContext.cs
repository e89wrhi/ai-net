using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ImageCaption.Models;

namespace ImageCaption.Data;

public class ImageCaptionReadDbContext : MongoDbContext
{
    public ImageCaptionReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        ImageCaptions = GetCollection<ImageCaptionReadModel>(nameof(ImageCaptions).Underscore());
    }

    public IMongoCollection<ImageCaptionReadModel> ImageCaptions { get; }
}