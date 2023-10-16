using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Models;

public class TestCase : ITestCase
{
    public string Id { get; set; }
    public string Input { get; set; }
    public string Output { get; set; }
}