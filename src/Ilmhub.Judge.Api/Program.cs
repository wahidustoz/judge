using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api;
using Ilmhub.Judge.Messaging;
using Ilmhub.Judge.Messaging.Shared.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidators();
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration);
builder.Services.AddLogging(o => o.ConfigureOpenTelemetryLogging(builder.Configuration));
builder.Services.AddHealthChecks().AddCheck<CompilerHealthChecks>("Compilers");
builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.AddJudgeMessaging(builder.Configuration, options => 
{
    options.AddMasstransitBus((config, provider) =>
    {
        config.RegisterConsumer<RunCommand>(provider);
        config.RegisterConsumer<JudgeCommand>(provider);
    });
    options.RegisterCommandHandler<RunCommand, RunCommandHandler>();
    options.RegisterCommandHandler<JudgeCommand, JudgeCommandHandler>();
});

var app = builder.Build();
app.UseRateLimiter();
app.UseEndpoints();
app.ConfigureHealthChecks();
app.Run();