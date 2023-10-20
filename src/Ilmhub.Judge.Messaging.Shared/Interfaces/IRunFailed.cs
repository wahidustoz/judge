namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IRunFailed : IJudgeEvent {
    string Error { get; set; }
}