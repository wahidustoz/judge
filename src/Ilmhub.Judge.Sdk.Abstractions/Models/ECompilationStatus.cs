namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public enum ECompilationStatus
{
    Success,
    SyntaxError,
    MemoryLimit,
    CpuLimit,
    RealTimeLimit,
    SystemError
}
