namespace Ilmhub.Judge.Client.Abstractions;

public interface IJudgeServerOptions
{
    string BaseUrl { get; set; }
    string Token { get; set; }
}
