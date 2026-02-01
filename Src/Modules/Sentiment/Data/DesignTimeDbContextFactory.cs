using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sentiment.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SentimentDbContext>
{
    public SentimentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SentimentDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=sentiment;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new SentimentDbContext(builder.Options);
    }
}