using AI.Common.Mongo;
using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Payment.Models;

namespace Payment.Data;

public class PaymentReadDbContext : MongoDbContext
{
    public PaymentReadDbContext(IOptions<MongoOptions> options) : base(options)
    {
        Subscription = GetCollection<SubscriptionReadModel>(nameof(Subscription).Underscore());
    }

    public IMongoCollection<SubscriptionReadModel> Subscription { get; }
}