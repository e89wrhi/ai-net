using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageCaption.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ImageCaptionDbContext>
{
    public ImageCaptionDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ImageCaptionDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=imagecaption;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ImageCaptionDbContext(builder.Options);
    }
}