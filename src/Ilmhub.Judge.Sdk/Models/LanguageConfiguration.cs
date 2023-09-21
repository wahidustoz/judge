using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class LanguageConfiguration : ILanguageConfiguration
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICompileConfiguration Compile { get; set; } = new CompileConfiguration();
    public IRunConfiguration Run { get; set; } = new RunConfiguration();
}