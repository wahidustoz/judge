namespace Ilmhub.Judge.Wrapper.Abstractions.Models;

public enum EExecutionError
{
    Success = 0,              
    InvalidConfig = -1,
    ForkFailed = -2,
    PthreadFailed = -3,
    WaitFailed = -4,
    RootRequired = -5,
    LoadSeccompFailed = -6,
    SetrlimitFailed = -7,
    Dup2Failed = -8,
    SetuidFailed = -9,
    ExecveFailed = -10,
    SpjError = -11            // Used for answer checking
}

