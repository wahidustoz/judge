using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudger
{
    bool TestCaseExists(Guid testCaseId);
    string GetTestCaseFolder(Guid testCaseId);
    ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source, 
        IEnumerable<ITestCase> testCases, 
        bool? useStrictMode = default,
        long maxCpu = -1,
        long maxMemory = -1,
        string environmentFolder = default,
        CancellationToken cancellationToken = default);

    ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source, 
        Guid testCaseId,
        bool? useStrictMode = default, 
        long maxCpu = -1,
        long maxMemory = -1,
        string environmentFolder = default,
        CancellationToken cancellationToken = default);
    
    ValueTask<Guid> CreateTestCaseAsync(IEnumerable<ITestCase> testCases, CancellationToken cancellationToken = default); 
    Guid CreateTestCaseFromZipArchive(Stream zipStream);
}
