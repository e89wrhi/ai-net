using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeGen.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CodeGenDbContext>
{
    public CodeGenDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CodeGenDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=codegen;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new CodeGenDbContext(builder.Options);
    }
}