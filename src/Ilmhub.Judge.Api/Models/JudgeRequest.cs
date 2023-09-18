namespace Ilmhub.Judge.Api.Models;

public class JudgeRequest
{
    public int LanguageId { get; set; }
    public string Source { get; set; }
    public IEnumerable<Testcase> Testcases { get; set; }
}