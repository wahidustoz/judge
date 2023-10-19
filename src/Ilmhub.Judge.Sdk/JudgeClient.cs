using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Sdk;

public class JudgeClient : IJudgeClient
{
    public ValueTask<Guid> AddTestCasesAsync(IEnumerable<TestCase> testCases, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Language> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<JudgeResult> JudgeAsync(string sourceCode, int languageId, Guid testCasesId, long? maxCpu = null, long? maxMemory = null, bool? useStrictMode = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<JudgeResult> JudgeAsync(string sourceCode, int languageId, IEnumerable<TestCase> testCases, long? maxCpu = null, long? maxMemory = null, bool? useStrictMode = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<RunResult> RunAsync(string sourceCode, int languageId, IEnumerable<string> inputs, long? maxCpu = null, long? maxMemory = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<RunResult> RunAsync(string sourceCode, int languageId, Guid testCasesId, long? maxCpu = null, long? maxMemory = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
