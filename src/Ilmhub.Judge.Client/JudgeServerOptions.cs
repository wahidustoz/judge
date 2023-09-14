using Ilmhub.Judge.Client.Abstractions;

namespace Ilmhub.Judge.Client;

public class JudgeServerOptions : IJudgeServerOptions
{
    public string BaseUrl { get; set; }
    public string Token { get; set; }
}
