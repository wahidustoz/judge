using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIlmhubJudge(builder.Configuration.GetSection($"{IlmhubJudgeOptions.Name}"));

var app = builder.Build();

app.MapGet("/languages", async (ILanguageService languageService, CancellationToken cancellationToken) 
    => await languageService.GetLanguagesAsync(cancellationToken)).WithName("Languages");

app.Run();