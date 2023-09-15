using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Models;

public class TestCase : ITestCase
{
    public string Input { get; set; }
    public string Output { get; set; }
}