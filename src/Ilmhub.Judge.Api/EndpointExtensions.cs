using System.Net;
using FluentValidation;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Sdk.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ilmhub.Judge.Api;

public static class EndpointExtensions
{
    public static WebApplication AddEndpoints(this WebApplication app)
    {
        app.MapPost("/judge", async (IJudger judger, IIlmhubJudgeOptions options, JudgeRequestDto dto, CancellationToken cancellationToken) =>
        {
            IJudgeResult judgeResult = dto.TestCases?.Any() is true
            ? await judger.JudgeAsync(dto.LanguageId, dto.Source, dto.TestCases, dto.MaxCpu, dto.MaxMemory, cancellationToken: cancellationToken)
            : await judger.JudgeAsync(dto.LanguageId, dto.Source, dto.TestCaseId, dto.MaxCpu, dto.MaxMemory, cancellationToken: cancellationToken);

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
        .AddEndpointFilter<ValidateFilter>()
        .WithName("Judge");

        app.MapGet("/languages", async (ILanguageService service, CancellationToken token) =>
            await service.GetLanguagesAsync(token)).WithName("Languages");

        return app;
    }
}

public class ValidateFilter : IEndpointFilter
{
    private readonly IValidator<JudgeRequestDto> _validator;
    public ValidateFilter(IValidator<JudgeRequestDto> validator)
    {
        this._validator = validator;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var judgeResult = context.Arguments.FirstOrDefault(a => a.GetType() == typeof(JudgeRequestDto)) as JudgeRequestDto;
        var result = await _validator.ValidateAsync(judgeResult);
        if (!result.IsValid) return Results.Json(result.Errors, statusCode: (int)HttpStatusCode.BadRequest);
        return await next(context);
        
    }

}
