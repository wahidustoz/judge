using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudger
{
    ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source, 
        long maxCpu,
        long maxMemory,
        IEnumerable<ITestCase> testCases, 
        CancellationToken cancellationToken = default);

    ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source, 
        long maxCpu,
        long maxMemory,
        string testCasesFolder, 
        string environmentFolder = default,
        CancellationToken cancellationToken = default);
}
