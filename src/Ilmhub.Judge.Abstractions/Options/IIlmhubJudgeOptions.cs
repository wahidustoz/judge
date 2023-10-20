using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions.Options;

public interface IIlmhubJudgeOptions {
    string RootFolder { get; }
    IJudgeUsersOption SystemUsers { get; set; }
    IEnumerable<ILanguageConfiguration> LanguageConfigurations { get; set; }
}