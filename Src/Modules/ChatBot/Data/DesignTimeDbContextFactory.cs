using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatBot.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ChatDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=chat_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new ChatDbContext(builder.Options);
    }
}
