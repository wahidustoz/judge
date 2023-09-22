using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));
builder.Services.AddTransient<JudgeService>();

var app = builder.Build();

app.MapGet("/languages", async (ILanguageService service, CancellationToken token) => await service.GetLanguagesAsync(token)).WithName("Languages");
app.MapPost("/judge", async (JudgeRequestDto request, JudgeService service, CancellationToken cancellationToken) =>
    await service.JudgeAsync(request, cancellationToken)).WithName("Judge");
app.Run();