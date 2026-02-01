using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageEdit.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ImageEditDbContext>
{
    public ImageEditDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ImageEditDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=imageedit;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ImageEditDbContext(builder.Options);
    }
}