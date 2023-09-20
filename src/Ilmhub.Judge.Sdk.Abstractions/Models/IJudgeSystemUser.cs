namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeSystemUser
{
    string Username { get; }
    long UserId { get; }
    long GroupId { get; }
}
