using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SpeechToText.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpeechToTextDbContext>
{
    public SpeechToTextDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SpeechToTextDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=speechtotext;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new SpeechToTextDbContext(builder.Options);
    }
}