namespace Ilmhub.Judge.Sdk.Models;

public enum EExecutionStatus
{
    Success,
    CpuTimeLimitExceeded,
    RealTimeLimitExceeded,
    MemoryLimitExceeded,
    RuntimeError,
    SystemError
}