namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public enum ECompileErrorStatus
{
    Syntax,
    CpuTimeLimit = 1,
    RealTimeLimit = 2,
    MemoryLimit = 3
}