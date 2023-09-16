using Ilmhub.Judge.Sdk.Abstractions;

namespace Ilmhub.Judge.Sdk;

public class JudgeServerOptions : IJudgeServerOptions
{
    public string BaseUrl { get; set; }
    public string Token { get; set; }
}
