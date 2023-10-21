using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Models;

namespace Ilmhub.Judge.Options;

public class JudgeUsersOption : IJudgeUsersOption
{
    public IJudgeSystemUser Compiler { get; set; } = new JudgeSystemUser();
    public IJudgeSystemUser Runner { get; set; } = new JudgeSystemUser();
}