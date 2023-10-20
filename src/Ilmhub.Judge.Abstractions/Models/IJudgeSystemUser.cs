namespace Ilmhub.Judge.Abstractions.Models;

public interface IJudgeSystemUser {
    string Username { get; }
    long UserId { get; }
    long GroupId { get; }
}