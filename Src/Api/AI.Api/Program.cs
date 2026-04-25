using AI.Common.Web;
using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure();

// Automated discovery for IModule implementations (Identity and ChatBot)
builder.AddModules(); 

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Automatically configures middleware for discovered modules (Identity and ChatBot)
app.UseModules(); 

app.UserSharedInfrastructure();
app.MapMinimalEndpoints();

app.Run();

namespace Api
{
    public partial class Program
    {
    }
}