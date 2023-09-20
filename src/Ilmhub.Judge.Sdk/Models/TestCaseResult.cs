
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class TestCaseResult : ITestCaseResult
{
    public string Id { get; set; }
    public string OutputMd5 { get; set; }
    public string Output { get; set; }
    public IExecutionResult Execution { get; set; }
}