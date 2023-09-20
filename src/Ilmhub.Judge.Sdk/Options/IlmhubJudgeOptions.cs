using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Options;

public class IlmhubJudgeOptions : IIlmhubJudgeOptions
{
    public IJudgeUsersOption SystemUsers { get; set; }
    public IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; } = new List<ILanguageConfiguration>();
}