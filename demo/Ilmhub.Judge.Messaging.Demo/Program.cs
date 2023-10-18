using Ilmhub.Judge.Messaging.Demo;
using Ilmhub.Judge.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddIlmhubJudge(builder.Configuration, o => o.AddJudgeEventHandler<JudgeEventHandler>());
builder.Services.AddHostedService<PeriodicJudgeCommandSender>();

var app = builder.Build();

app.Run();