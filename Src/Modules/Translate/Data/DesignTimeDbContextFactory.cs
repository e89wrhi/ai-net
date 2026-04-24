using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Translate.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TranslateDbContext>
{
    public TranslateDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TranslateDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=translate_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new TranslateDbContext(builder.Options);
    }
}
