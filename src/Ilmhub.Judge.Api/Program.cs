using Ilmhub.Judge.Api.Services;
using Ilmhub.Judge.Api.Options;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<FileService>();
builder.Services.AddTransient<JudgeService>();
builder.Services.AddTransient<RunnerService>();
builder.Services.AddTransient<LanguageService>();

builder.Services.Configure<JudgeEnvironmentOptions>(builder.Configuration.GetSection("Judge"));

var app = builder.Build();

app.MapGet("/say-hello", () => "Hello World!");

await app.Services.GetRequiredService<JudgeService>().JudgeAsync(new Ilmhub.Judge.Api.Models.JudgeRequest
{
    LanguageId = 1,
    Source = "int main() { int a, b; scanf(\" %d %d\", &a, &b); printf(\"%d\", a+b); return 0;}",
    Testcases = new List<Ilmhub.Judge.Api.Models.Testcase>
    {
        new Ilmhub.Judge.Api.Models.Testcase { Input = "1 2", Output = "3" },
        new Ilmhub.Judge.Api.Models.Testcase { Input = "4 5", Output = "0" }
    }
});

app.Run();