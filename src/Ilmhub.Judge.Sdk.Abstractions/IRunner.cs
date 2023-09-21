using Ilmhub.Judge.Sdk.Abstraction.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IRunner
{
    ValueTask<IRunnerResult> RunAsync(
        int languageId, 
        string executableFilename, 
        long maxCpu,
        long maxMemory,
        string input = default, 
        string environmentFolder = default,
        CancellationToken cancellationToken = default);

        ValueTask<IRunnerResult> RunAsync(
        int languageId, 
        string inputFilePath,
        string executableFilePath, 
        long maxCpu,
        long maxMemory,
        string environmentFolder = default,
        CancellationToken cancellationToken = default);
}
