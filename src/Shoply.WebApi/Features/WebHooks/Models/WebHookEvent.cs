using Shoply.WebApi.Common.Shared.Models;

namespace Shoply.WebApi.Features.WebHooks.Models;

[ValueObject<Guid>]
public readonly partial struct WebHookEventId;

public sealed class WebHookEvent : Entity<WebHookEventId>
{
    private WebHookEvent()
    {
    }

    public WebHookEvent(WebHookEventType eventType, string eventId, string payload)
    {
        Id = WebHookEventId.From(Guid.CreateVersion7());
        EventType = eventType;
        EventId = eventId;
        Status = WebHookEventStatus.Pending;
        Payload = payload;
        RetryCount = 0;
    }

    public WebHookEventType EventType { get; init; }
    public WebHookEventStatus Status { get; private set; }
    public string EventId { get; init; }
    public string Payload { get; init; }
    public int RetryCount { get; private set; }

    public void IncrementRetryCount()
    {
        RetryCount++;
    }

    public void MarkFailed()
    {
        Status = WebHookEventStatus.Failed;
    }

    public void MarkHandled()
    {
        Status = WebHookEventStatus.Handled;
    }
}