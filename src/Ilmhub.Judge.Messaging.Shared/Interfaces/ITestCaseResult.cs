namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface ITestCaseResult : IRunResult
{
    string Id { get; set; }
    string Status { get; set; }

}