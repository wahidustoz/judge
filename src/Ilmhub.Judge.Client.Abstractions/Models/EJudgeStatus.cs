namespace Ilmhub.Judge.Client.Abstractions.Models;

public enum EJudgeStatus
{
    WrongAnswer = -1,
    Success = 0,
    CpuTimeLimitExceeded = 1,
    RealTimeLimitExceeded = 2,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    SystemError = 5
}