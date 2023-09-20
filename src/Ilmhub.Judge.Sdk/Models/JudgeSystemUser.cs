using Ilmhub.Judge.Sdk.Abstractions;

namespace Ilmhub.Judge.Sdk;

public class JudgeSystemUser : IJudgeSystemUser
{
    public JudgeSystemUser(string username, long userId, long groupId)
    {
        Username = username;
        UserId = userId;
        GroupId = groupId;
    }

    public string Username { get; }

    public long UserId { get; }

    public long GroupId { get; }
}
