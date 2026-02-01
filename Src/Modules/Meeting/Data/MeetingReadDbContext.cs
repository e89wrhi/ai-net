using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Meeting.Models;

namespace Meeting.Data;

public class MeetingReadDbContext : MongoDbContext
{
    public MeetingReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Meeting = GetCollection<MeetingAnalysisSessionReadModel>(nameof(Meeting).Underscore());
    }

    public IMongoCollection<MeetingAnalysisSessionReadModel> Meeting { get; }
}