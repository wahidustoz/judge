namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public enum EExecutionResult
{
    Success = 0, 
    CpuTimeLimitExceeded = 1,
    RealTimeLimitExceeded = 2,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    SystemError = 5
}