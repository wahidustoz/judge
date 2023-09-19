namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ICompileSpecialRequest
{
    string Version { get; set; }
    string SourceCode { get; set; }
    ICompileConfiguration Configuration { get; set; }
}
