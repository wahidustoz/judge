namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCase
{
    string Id { get; set; }
    string Input { get; set; }
    string Output { get; set; }
}
