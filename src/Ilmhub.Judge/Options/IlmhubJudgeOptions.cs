using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;

namespace Ilmhub.Judge.Options;

public class IlmhubJudgeOptions : IIlmhubJudgeOptions
{
    public static string Name => "Judger";
    public string RootFolder { get; set; } = "/judger";
    public IJudgeUsersOption SystemUsers { get; set; } = new JudgeUsersOption();
    public IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}