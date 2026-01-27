using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Meeting.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MeetingDbContext>
{
    public MeetingDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MeetingDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=meeting;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new MeetingDbContext(builder.Options);
    }
}