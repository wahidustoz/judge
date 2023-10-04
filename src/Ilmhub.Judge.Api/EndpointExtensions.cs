using FluentValidation;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Api;

public static class EndpointExtensions
{
    public static WebApplication AddEndpoints(this WebApplication app)
    {
        app.MapPost("/judge", async (
            IJudger judger,
            IIlmhubJudgeOptions options,
            JudgeRequestDto dto,
            CancellationToken cancellationToken) =>
        {
            IJudgeResult judgeResult = dto.TestCases?.Any() is true
            ? await judger.JudgeAsync(
                languageId: dto.LanguageId,
                source: dto.Source,
                testCases: dto.TestCases,
                usestrictMode: dto.UseStrictMode,
                maxCpu: dto.MaxCpu ?? -1,
                maxMemory: dto.MaxMemory ?? -1,
                cancellationToken: cancellationToken)
            : await judger.JudgeAsync(
                languageId: dto.LanguageId,
                source: dto.Source,
                testCaseId: dto.TestCaseId.Value,
                usestrictMode: dto.UseStrictMode,
                maxCpu: dto.MaxCpu ?? -1,
                maxMemory: dto.MaxMemory ?? -1,
                cancellationToken: cancellationToken);

            return Results.Ok(new
            {
                IsSuccess = judgeResult.IsSuccess,
                Status = judgeResult.Status,
                StatusString = judgeResult.Status.ToString(),
                Compilation = new
                {
                    IsSuccess = judgeResult.Compilation?.IsSuccess,
                    Output = judgeResult.Compilation?.Output,
                    ErrorMessage = judgeResult.Compilation?.Error,
                    Status = judgeResult.Compilation?.Execution?.Status,
                    StatusString = judgeResult.Compilation?.Execution?.Status.ToString()
                },
                TestCases = judgeResult.TestCases?.Select(tc => new
                {
                    Id = tc.Id,
                    Status = tc.Status,
                    StatusString = tc.Status.ToString(),
                    Output = tc.Output,
                    OutputMd5 = tc.OutputMd5,
                    ExpectedOutput = tc.ExpectedOutput,
                    ExpectedOutputMd5 = tc.ExpectedOutputMd5,
                    CpuTime = tc.Execution?.Execution?.CpuTime,
                    Memory = tc.Execution?.Execution?.Memory,
                    RealTime = tc.Execution?.Execution?.RealTime
                })
            });
        })
        .WithAsyncValidation<JudgeRequestDto>()
        .WithRateLimiting("fixed", app.Configuration)
        .WithName("Judge");


        app.MapGet("/languages", async (
            ILanguageService service,
            CancellationToken token) => await service.GetLanguagesAsync(token))
        .WithRateLimiting("fixed", app.Configuration)
        .WithName("Languages");

        app.MapPost("/testcase", async (
            IJudger judger,
            List<TestCaseDto> dto,
            CancellationToken cancellationToken) =>
        {
            var testCaseId = await judger.CreateTestCaseAsync(
                testCases: dto.Select(tc => new TestCase
                {
                    Id = tc.Id,
                    Input = tc.Input,
                    Output = tc.Output
                }),
                cancellationToken: cancellationToken);

            return Results.Ok(testCaseId);
        })
        .WithAsyncValidation<IEnumerable<TestCaseDto>>()
        .WithRateLimiting("fixed", app.Configuration)
        .WithName("TestCases");

        app.MapPost("/testcase-files", async (
            IJudger judger,
            IFormFile testcases,
            CancellationToken cancellationToken) =>
            {
                return Results.Ok(judger.CreateTestCaseFromZipArchive(testcases.OpenReadStream()));
            })
        .WithAsyncValidation<IFormFile>()
        .WithRateLimiting("fixed", app.Configuration)
        .WithName("TestCase-Files");

        return app;
    }
}