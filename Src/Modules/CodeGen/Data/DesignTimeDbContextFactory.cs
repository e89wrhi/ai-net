using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeGen.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CodeGenDbContext>
{
    public CodeGenDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CodeGenDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=codegen_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new CodeGenDbContext(builder.Options);
    }
}
