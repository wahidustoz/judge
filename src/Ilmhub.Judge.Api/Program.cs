using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api.Services;
using Ilmhub.Judge.Api;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.AddTransient<JudgeService>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(b =>
    {
        b.AddOtlpExporter();
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ilmhub.Judge.Api"));
    })
    .WithTracing(b => 
    {
        b.AddOtlpExporter();
        b.AddAspNetCoreInstrumentation();
        b.AddHttpClientInstrumentation();
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ilmhub.Judge.Api"));
    });

    builder.Logging.ClearProviders();
    builder.Logging.AddOpenTelemetry(b =>
    {
        b.AddOtlpExporter();
        b.AttachLogsToActivityEvent();
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ilmhub.Judge.Api"));
    });
if(builder.Environment.IsDevelopment() is false)
{
}

var app = builder.Build();
app.AddEndpoints();
app.Run();