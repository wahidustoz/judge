using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCaseResult
{
    string Id { get; set; }
    string OutputMd5 { get; set; }
    string Output { get; set; }
    IExecutionResult Execution { get; set; }
}