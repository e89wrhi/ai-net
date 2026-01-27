using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Resume.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ResumeDbContext>
{
    public ResumeDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ResumeDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=resume;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ResumeDbContext(builder.Options);
    }
}