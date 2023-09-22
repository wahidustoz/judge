namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCase
{
    int Id { get; set; }
    string Input { get; set; }
    string Output { get; set; }
}
