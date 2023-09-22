using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Api.Services;

public class JudgeService
{
    private readonly IJudger judger;

    public JudgeService(IJudger judger) => this.judger = judger;

    public async ValueTask<JudgeResultDto> JudgeAsync(JudgeRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await judger.JudgeAsync(
            languageId: request.LanguageId,
            source: request.Source,
            maxCpu: request.MaxCpu,
            maxMemory: request.MaxMemory,
            testCases: request.TestCases,
            cancellationToken: cancellationToken);
        
        return new JudgeResultDto
        {
            IsSuccess = result.IsSuccess,
            ErrorMessage = result.Compilation.Error + result.Compilation.Output,
            Status = result.Status,
            TestCases = result.TestCases?.Select(tc => new TestCaseResultDto
            {
                Id = tc.Id,
                Status = tc.Status,
                Output = tc.Output,
                OutputMd5 = tc.OutputMd5,
                CpuUsage = tc.Execution.Execution.CpuTime,
                MemoryUsage = tc.Execution.Execution.Memory,
                RealtimeUsage = tc.Execution.Execution.RealTime
            }),

        };
    }
}

public class JudgeResultDto
{
    public bool IsSuccess { get; set; }
    public EJudgeStatus Status { get; set; }
    public IEnumerable<TestCaseResultDto> TestCases { get; set; }
    public string ErrorMessage { get; set; }
}

public class TestCaseResultDto
{
    public string Id { get; set; }
    public string Output { get; set; }
    public string OutputMd5 { get; set; }
    public long CpuUsage { get; set; }
    public long MemoryUsage { get; set; }
    public long RealtimeUsage { get; set; }
    public ETestCaseStatus Status { get; set; }
}