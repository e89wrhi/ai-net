using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ChatBot.Models;

namespace ChatBot.Data;

public class ChatReadDbContext : MongoDbContext
{
    public ChatReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Chats = GetCollection<ChatSessionReadModel>(nameof(Chats).Underscore());
    }

    public IMongoCollection<ChatSessionReadModel> Chats { get; }
}