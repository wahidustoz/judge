using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureRateLimiting(builder.Configuration);
builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.SetupOpenTelemetry(builder.Configuration);
builder.Services.AddLogging(logBuilder => logBuilder.ConfigureOpenTelemetryLogging(builder.Configuration));
builder.Services.SetupHealthChecks();

var app = builder.Build();
app.UseRateLimiter();
app.AddEndpoints();
app.ConfigureHealthChecks();
app.Run();