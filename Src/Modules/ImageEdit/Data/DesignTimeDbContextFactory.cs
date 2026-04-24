using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageEdit.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ImageEditDbContext>
{
    public ImageEditDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ImageEditDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=image_edit_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ImageEditDbContext(builder.Options);
    }
}
