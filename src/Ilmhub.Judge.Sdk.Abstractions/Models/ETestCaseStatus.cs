namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public enum ETestCaseStatus
{
    Success,
    WrongAnswer,
    /// <summary>
    /// Output has extra/missing white spaces at the start/end
    /// </summary>
    PresentationError, 
    RuntimeError, 
    CpuLimit,
    MemoryLimit,
    RealtimeLimit,
    SystemError
}