namespace AI.Common.EFCore;

/// <summary>
/// Orchestrates the data seeding process.
/// It finds all registered IDataSeeder implementations and runs them in the correct order.
/// </summary>
public interface ISeedManager
{
    Task ExecuteSeedAsync();
    Task ExecuteTestSeedAsync();
}