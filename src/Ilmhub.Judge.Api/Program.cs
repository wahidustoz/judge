using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
        "Too many requests. Please try again later.", cancellationToken: token);
    };
});

builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.SetupOpenTelemetry(builder.Configuration);
builder.Services.AddLogging(logBuilder => logBuilder.ConfigureOpenTelemetryLogging(builder.Configuration));
builder.Services.SetupHealthChecks();

var app = builder.Build();
app.UseRateLimiter();
app.AddEndpoints();
app.ConfigureHealthChecks();
app.Run();