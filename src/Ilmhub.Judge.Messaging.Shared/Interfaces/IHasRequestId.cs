namespace Ilmhub.Judge.Messaging.Shared.Interfaces;

public interface IHasRequestId
{
    Guid RequestId { get; set; }
}