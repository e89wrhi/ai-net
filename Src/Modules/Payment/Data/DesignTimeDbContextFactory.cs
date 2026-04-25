using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payment.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PaymentDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=payment_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new PaymentDbContext(builder.Options);
    }
}
