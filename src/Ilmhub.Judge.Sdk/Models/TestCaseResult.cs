
using Ilmhub.Judge.Sdk.Abstraction.Models;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class TestCaseResult : ITestCaseResult
{
    public TestCaseResult(string id, IRunnerResult execution)
    {
        Id = id;
        Execution = execution;
    }

    public string Id { get; set; }
    public string OutputMd5 { get; set; }
    public string Output { get; set; }
    public IRunnerResult Execution { get; set; }
}