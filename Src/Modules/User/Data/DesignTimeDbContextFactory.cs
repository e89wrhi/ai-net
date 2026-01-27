using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace User.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UserDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=user;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new UserDbContext(builder.Options);
    }
}