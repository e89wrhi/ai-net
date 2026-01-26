namespace AI.Common.EFCore;

/// <summary>
/// A simple configuration object to hold our PostgreSQL connection string.
/// This is typical used with the "Options Pattern" to pull settings from appsettings.json.
/// </summary>
public class PostgresOptions
{
    public string ConnectionString { get; set; }
}