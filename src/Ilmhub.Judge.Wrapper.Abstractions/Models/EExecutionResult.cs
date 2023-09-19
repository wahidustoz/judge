namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public enum EExecutionResult
{
    WrongAnswer = -1,            // Used for answer checking
    Success = 0,                // Process exited normally
    CpuTimeLimitExceeded = 1,
    RealTimeLimitExceeded = 2,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    SystemError = 5
}
