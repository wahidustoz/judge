namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeServerOptions
{
    string BaseUrl { get; set; }
    string Token { get; set; }
}
