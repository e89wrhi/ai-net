using AI.Common.Web;
using Api.Extensions;
using AutoComplete.Extensions;
using ChatBot.Extensions;
using CodeDebug.Extensions;
using CodeGen.Extensions;
using Identity.Extensions.Infrastructure;
using ImageCaption.Extensions;
using ImageEdit.Extensions;
using ImageGen.Extensions;
using LearningAssistant.Extensions;
using Meeting.Extensions;
using Payment.Extensions;
using Resume.Extensions;
using SimpleMD.Extensions;
using SimplePlugin.Extensions;
using SpeechToText.Extensions;
using Summary.Extensions;
using TextToSpeech.Extensions;
using Translate.Extensions;
using User.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure();

builder.AddIdentityModules();
builder.AddAutoCompleteModules();
builder.AddChatModules();
builder.AddCodeDebugModules();
builder.AddCodeGenModules();
builder.AddImageModules();
builder.AddImageEditModules();
builder.AddImageGenModules();
builder.AddMeetingModules();
builder.AddAssistantModules();
builder.AddPaymentModules();
builder.AddResumeModules();
builder.AddSimpleMDModules();
builder.AddSimplePluginModules();
builder.AddSpeechToTextModules();
builder.AddTextToSpeechModules();
builder.AddSummaryModules();
builder.AddTranslateModules();
builder.AddUserModules();

var app = builder.Build();

// ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#routing-basics
app.UseAuthentication();
app.UseAuthorization();


app.UseIdentityModules();
app.UseAutoCompleteModules();
app.UseChatModules();
app.UseCodeDebugModules();
app.UseCodeGenModules();
app.UseImageModules();
app.UseImageEditModules();
app.UseImageGenModules();
app.UseMeetingModules();
app.UseAssistantModules();
app.UsePaymentModules();
app.UseResumeModules();
app.UseSimpleMDModules();
app.UseSimplePluginModules();
app.UseSpeechToTextModules();
app.UseTextToSpeechModules();
app.UseSummaryModules();
app.UseTranslateModules();
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