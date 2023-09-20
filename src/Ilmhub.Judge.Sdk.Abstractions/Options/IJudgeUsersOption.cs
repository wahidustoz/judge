namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeUsersOption
{
    IJudgeSystemUser Compiler { get; set; }
    IJudgeSystemUser Runner { get; set; }
}