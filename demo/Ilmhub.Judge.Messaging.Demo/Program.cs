using Ilmhub.Judge.Messaging.Demo;
using Ilmhub.Judge.Sdk;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIlmhubJudge(builder.Configuration, o => o
    .AddCommandPublisher()
    .AddJudgeEventHandler<JudgeEventHandler>()
    .AddJudgeClient());
builder.Services.AddHostedService<PeriodicJudgeCommandSender>();

var app = builder.Build();

await app.RunAsync();