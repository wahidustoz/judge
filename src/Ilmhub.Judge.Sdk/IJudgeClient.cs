using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Sdk;
public interface IJudgeClient
{
    ValueTask<Language> GetLanguagesAsync(CancellationToken cancellationToken = default);
    ValueTask<Guid> AddTestCasesAsync(IEnumerable<TestCase> testCases, CancellationToken cancellationToken = default);
    ValueTask<JudgeResult> JudgeAsync(
        string sourceCode,
        int languageId,
        Guid testCasesId,
        long? maxCpu = default,
        long? maxMemory = default,
        bool? useStrictMode = default,
        CancellationToken cancellationToken = default);
    ValueTask<JudgeResult> JudgeAsync(
        string sourceCode,
        int languageId,
        IEnumerable<TestCase> testCases,
        long? maxCpu = default,
        long? maxMemory = default,
        bool? useStrictMode = default,
        CancellationToken cancellationToken = default);
    ValueTask<RunResult> RunAsync(
        string sourceCode,
        int languageId,
        IEnumerable<string> inputs,
        long? maxCpu = default,
        long? maxMemory = default,
        CancellationToken cancellationToken = default);
    ValueTask<RunResult> RunAsync(
        string sourceCode,
        int languageId,
        Guid testCasesId,
        long? maxCpu = default,
        long? maxMemory = default,
        CancellationToken cancellationToken = default);
}