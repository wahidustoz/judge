namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface IJudgeSpecialRequest : IJudgeRequest
{
    string SpecialSourceCode { get; set; }
    string Version { get; set; }
    ICompileConfiguration SpecialCompileConfiguration { get; set; }
    IRunConfiguration SpecialJudgeConfiguration { get; set; }
}