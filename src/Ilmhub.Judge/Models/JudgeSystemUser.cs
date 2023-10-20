using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Models;

public class JudgeSystemUser : IJudgeSystemUser {
    public JudgeSystemUser() { }
    public JudgeSystemUser(string username, long userId, long groupId) {
        Username = username;
        UserId = userId;
        GroupId = groupId;
    }

    public string Username { get; set; }
    public long UserId { get; set; }
    public long GroupId { get; set; }
}