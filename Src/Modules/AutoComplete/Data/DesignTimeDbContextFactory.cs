using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoComplete.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AutocompleteDbContext>
{
    public AutocompleteDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AutocompleteDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=autocomplete;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new AutocompleteDbContext(builder.Options);
    }
}