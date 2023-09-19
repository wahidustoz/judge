namespace Ilmhub.Judge.Sdk.Abstractions.Models;

public interface ITestCase
{
    string Input { get; set; }
    string Output { get; set; }
}
