using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Globalization;

namespace AI.Common.Mongo;

/// <summary>
/// A concrete implementation of IMongoDbContext.
/// Manages connection to MongoDB, collection retrieval, and transaction lifecycle.
/// </summary>
public class MongoDbContext : IMongoDbContext
{
    /// <summary>
    /// The active MongoDB session, if any.
    /// </summary>
    public IClientSessionHandle? Session { get; set; }

    /// <summary>
    /// The underlying MongoDB database object.
    /// </summary>
    public IMongoDatabase Database { get; }

    /// <summary>
    /// The underlying MongoDB client object.
    /// </summary>
    public IMongoClient MongoClient { get; }

    /// <summary>
    /// A list of commands (funcs) to be executed when SaveChangesAsync is called.
    /// </summary>
    protected readonly IList<Func<Task>> _commands;
    private static readonly bool _isSerializerRegisterd;

    static MongoDbContext()
    {
        // Register Guid serializer to store Guids as strings by default
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    }

    public MongoDbContext(IOptions<MongoOptions> options)
    {
        RegisterConventions();

        MongoClient = new MongoClient(options.Value.ConnectionString);
        var databaseName = options.Value.DatabaseName;
        Database = MongoClient.GetDatabase(databaseName);

        // Every command will be stored and it'll be processed at SaveChanges
        _commands = new List<Func<Task>>();
    }

    /// <summary>
    /// Registers common MongoDB conventions like CamelCase element names and Enum representation.
    /// </summary>
    private static void RegisterConventions()
    {
        ConventionRegistry.Register(
            "conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfDefaultConvention(false),
                new ImmutablePocoConvention()
            }, _ => true);
    }

    /// <summary>
    /// Gets a collection of the specified type.
    /// The collection name defaults to the lowercase version of the type name.
    /// </summary>
    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        return Database.GetCollection<T>(name ?? typeof(T).Name.ToLower(CultureInfo.CurrentCulture));
    }

    public void Dispose()
    {
        while (Session is { IsInTransaction: true })
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Executes all cached commands within a single session and transaction.
    /// Returns the number of commands executed.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = _commands.Count;

        using (Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken))
        {
            Session.StartTransaction();

            try
            {
                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                await Session.AbortTransactionAsync(cancellationToken);
                _commands.Clear();
                throw;
            }
        }

        _commands.Clear();
        return result;
    }

    /// <summary>
    /// Manually begins a new session and transaction.
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        Session.StartTransaction();
    }

    /// <summary>
    /// Commits the active manual transaction.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Session is { IsInTransaction: true })
            await Session.CommitTransactionAsync(cancellationToken);

        Session?.Dispose();
    }

    /// <summary>
    /// Aborts the active manual transaction.
    /// </summary>
    public async Task RollbackTransaction(CancellationToken cancellationToken = default)
    {
        await Session?.AbortTransactionAsync(cancellationToken)!;
    }

    /// <summary>
    /// Queues a command for execution during the next save.
    /// </summary>
    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    /// <summary>
    /// Executes a functional block within a transactional scope.
    /// </summary>
    public async Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action();

            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes a functional block returning a result within a transactional scope.
    /// </summary>
    public async Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();

            await CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
    }
}