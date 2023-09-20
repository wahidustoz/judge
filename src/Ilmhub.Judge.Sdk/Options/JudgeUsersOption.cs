using Ilmhub.Judge.Sdk.Abstractions;

namespace Ilmhub.Judge.Sdk;

public class JudgeUsersOption : IJudgeUsersOption
{
    public IJudgeSystemUser Compiler { get; set; }
    public IJudgeSystemUser Runner { get; set; }
}