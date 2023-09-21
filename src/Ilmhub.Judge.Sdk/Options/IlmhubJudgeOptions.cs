using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Sdk.Options;

public class IlmhubJudgeOptions : IIlmhubJudgeOptions
{
    public static string Name => "Judge";
    public IJudgeUsersOption SystemUsers { get; set; } = new JudgeUsersOption();
    public IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}