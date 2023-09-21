using Ilmhub.Judge.Sdk.Abstraction.Models;

namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCaseResult
{
    string Id { get; set; }
    string OutputMd5 { get; set; }
    string Output { get; set; }
    IRunnerResult Execution { get; set; }
}