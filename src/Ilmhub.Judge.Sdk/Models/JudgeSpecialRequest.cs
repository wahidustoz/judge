using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class JudgeSpecialRequest : JudgeRequest, IJudgeSpecialRequest 
{
    public string SpecialSourceCode { get; set; }
    public string Version { get; set; }
    public ICompileConfiguration SpecialCompileConfiguration { get; set; }
    public IRunConfiguration SpecialJudgeConfiguration { get; set; }
}