using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions.Options;

public interface IJudgeUsersOption
{
    IJudgeSystemUser Compiler { get; set; }
    IJudgeSystemUser Runner { get; set; }
}