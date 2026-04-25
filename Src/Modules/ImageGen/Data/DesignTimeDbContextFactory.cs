using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageGen.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ImageGenDbContext>
{
    public ImageGenDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ImageGenDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=image_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ImageGenDbContext(builder.Options);
    }
}
