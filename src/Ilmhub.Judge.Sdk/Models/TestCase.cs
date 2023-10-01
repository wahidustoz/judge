using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class TestCase : ITestCase
{
    public string Id { get; set; }
    public string Input { get; set; }
    public string Output { get; set; }
}