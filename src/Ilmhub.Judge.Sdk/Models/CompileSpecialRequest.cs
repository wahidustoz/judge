using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class CompileSpecialRequest : ICompileSpecialRequest
{
    public string Version { get; set; }
    public string SourceCode { get; set; }
    public ICompileConfiguration Configuration { get; set; }
}