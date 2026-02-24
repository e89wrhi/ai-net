using AI.Common.Web;
using Payment.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Payment;

public class PaymentModule : IModule
{
    public WebApplicationBuilder AddModule(WebApplicationBuilder builder)
    {
        return builder.AddPaymentModules();
    }

    public WebApplication UseModule(WebApplication app)
    {
        return app.UsePaymentModules();
    }
}
