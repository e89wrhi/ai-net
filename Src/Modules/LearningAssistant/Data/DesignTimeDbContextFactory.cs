using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LearningAssistant.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LearningDbContext>
{
    public LearningDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<LearningDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=learning;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new LearningDbContext(builder.Options);
    }
}