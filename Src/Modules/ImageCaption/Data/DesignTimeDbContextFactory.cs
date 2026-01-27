using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageCaption.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ImageDbContext>
{
    public ImageDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ImageDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=image;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ImageDbContext(builder.Options);
    }
}