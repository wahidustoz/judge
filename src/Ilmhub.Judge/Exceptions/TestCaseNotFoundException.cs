namespace Ilmhub.Judge.Exceptions;

public class TestCaseNotFoundException : Exception
{
    public TestCaseNotFoundException(Guid testCaseId)
        : base($"Testcase with id {testCaseId} not found") { }
}
