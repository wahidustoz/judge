namespace Ilmhub.Judge.Sdk.Models;

public record TestCaseExecution(
    string Id,
    EExecutionStatus Status,
    string Output,
    string OutputMd5,
    long CputTime,
    long RealTime,
    long Memory);