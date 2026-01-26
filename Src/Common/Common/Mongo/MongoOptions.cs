namespace AI.Common.Mongo;

/// <summary>
/// Settings for connecting to MongoDB.
/// We need both the ConnectionString and the specific DatabaseName to work with.
/// </summary>
public class MongoOptions
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public static Guid UniqueId { get; set; } = Guid.NewGuid();
}