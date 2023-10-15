namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IRunCommand : ICommand
{
    int LanguageId { get; set; }
    string SourceCode { get; set; }
    IEnumerable<string> Inputs { get; set; }
    long? MaxCpu { get; set; }
    long? MaxMemory { get; set; }
}