namespace Ilmhub.Judge.Api.Dtos;

public class RunRequestDto
{
    public int LanguageId { get; set; }
    public string Source { get; set; }
    public long? MaxCpu { get; set; }
    public long? MaxMemory { get; set; }
    public IEnumerable<string> Inputs { get; set; }
    

}