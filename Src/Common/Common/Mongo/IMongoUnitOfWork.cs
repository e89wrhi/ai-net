namespace AI.Common.Mongo;

/// <summary>
/// A specialized Unit of Work interface for MongoDB.
/// It wraps the MongoDbContext to coordinate multiple operations as a single unit.
/// </summary>
public interface IMongoUnitOfWork<out TContext> : IUnitOfWork<TContext> where TContext : class, IMongoDbContext
{
}