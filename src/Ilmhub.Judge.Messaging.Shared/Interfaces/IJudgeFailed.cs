namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IJudgeFailed : IJudgeEvent {
    string Error { get; set; }
}