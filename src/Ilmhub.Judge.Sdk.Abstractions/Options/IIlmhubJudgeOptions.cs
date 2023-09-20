using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IIlmhubJudgeOptions
{
    IJudgeUsersOption SystemUsers { get; set; }
    IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}
