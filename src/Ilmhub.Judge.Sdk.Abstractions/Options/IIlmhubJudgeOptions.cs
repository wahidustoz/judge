using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IIlmhubJudgeOptions
{
    string RootFolder { get; }
    IJudgeUsersOption SystemUsers { get; set; }
    IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}
