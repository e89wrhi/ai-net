using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TextToSpeech.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TextToSpeechDbContext>
{
    public TextToSpeechDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TextToSpeechDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=texttospeech;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new TextToSpeechDbContext(builder.Options);
    }
}