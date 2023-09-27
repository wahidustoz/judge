using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Api.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IValidator<JudgeRequestDto>, JudgeRequestValidator>();
builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.SetupOpenTelemetry(builder.Configuration);
builder.Services.AddLogging(logBuilder => logBuilder.ConfigureOpenTelemetryLogging(builder.Configuration));
builder.Services.SetupHealthChecks();

var app = builder.Build();
app.AddEndpoints();
app.ConfigureHealthChecks();
app.Run();