using AI.Common.Web;
using Api.Extensions;
using ChatBot.Extensions;
using Identity.Extensions.Infrastructure;
using ImageCaption.Extensions;
using LearningAssistant.Extensions;
using Meeting.Extensions;
using Payment.Extensions;
using Resume.Extensions;
using User.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure();

builder.AddIdentityModules();
builder.AddChatModules();
builder.AddImageModules();
builder.AddMeetingModules();
builder.AddAssistantModules();
builder.AddPaymentModules();
builder.AddResumeModules();
builder.AddUserModules();

var app = builder.Build();

// ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#routing-basics
app.UseAuthentication();
app.UseAuthorization();

app.UseChatModules();
app.UseIdentityModules();
app.UseImageModules();
app.UseMeetingModules();
app.UseAssistantModules();
app.UsePaymentModules();
app.UseResumeModules();
app.UseUserModules();

app.UserSharedInfrastructure();
app.MapMinimalEndpoints();

app.Run();

namespace Api
{
    public partial class Program
    {
    }
}