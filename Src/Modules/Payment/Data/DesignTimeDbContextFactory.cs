using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payment.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PaymentDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=payment;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new PaymentDbContext(builder.Options);
    }
}