namespace Ilmhub.Judge.Sdk.Models;

public record Compilation(
    bool IsSuccess,
    string Output,
    string ErrorMessage,
    EExecutionStatus Status);