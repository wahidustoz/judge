using Ilmhub.Judge.Sdk.Abstraction.Models;

namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCaseResult
{
    string Id { get; }
    ETestCaseStatus Status { get; }
    string ExpectedOutput { get; }
    string ExpectedOutputMd5 { get; }
    string OutputMd5 { get; }
    string Output { get;  }
    IRunnerResult Execution { get;  }
}
