using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Summary.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SummaryDbContext>
{
    public SummaryDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SummaryDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=summary;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new SummaryDbContext(builder.Options);
    }
}