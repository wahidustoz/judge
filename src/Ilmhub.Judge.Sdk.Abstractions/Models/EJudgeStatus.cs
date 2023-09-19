namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public enum EJudgeStatus
{
    WrongAnswer = -1,
    Success = 0,
    LimitExceeded = 1,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    SystemError = 5
}