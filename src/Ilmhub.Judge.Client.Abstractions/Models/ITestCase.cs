namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface ITestCase
{
    string Input { get; set; }
    string Output { get; set; }
}
