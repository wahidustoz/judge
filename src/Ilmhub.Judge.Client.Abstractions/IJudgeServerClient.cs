using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Abstractions;

public interface IJudgeServerClient
{
    ValueTask<IServerInfo> PingAsync(CancellationToken cancellationToken = default);
    ValueTask<string> JudgeAsync(
        string sourceCode, 
        ILanguageConfiguration languageConfiguration,
        long maxCpuTime,
        long maxMemory,
        IEnumerable<ITestCase> testCases = default,
        string testCaseId = null,
        bool showsOutput = false,
        CancellationToken cancellationToken = default);
}
