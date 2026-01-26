namespace AI.Common.EFCore;

/// <summary>
/// Mark your data seeding classes with this interface.
/// It's used to populate the database with initial data (like admin users or lookup tables) when the app starts.
/// </summary>
public interface IDataSeeder
{
    Task SeedAllAsync();
}

public interface ITestDataSeeder
{
    Task SeedAllAsync();
}