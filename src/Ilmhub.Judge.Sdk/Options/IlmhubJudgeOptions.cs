using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Options;

public class IlmhubJudgeOptions : IIlmhubJudgeOptions
{
    public static string Name => "Judger";
    public string RootFolder { get; set; } = "/judger";
    public IJudgeUsersOption SystemUsers { get; set; } = new JudgeUsersOption();
    public IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}