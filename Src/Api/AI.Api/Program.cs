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