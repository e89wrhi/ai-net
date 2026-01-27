using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LearningAssistant.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AssistantDbContext>
{
    public AssistantDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AssistantDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=learning;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new AssistantDbContext(builder.Options);
    }
}