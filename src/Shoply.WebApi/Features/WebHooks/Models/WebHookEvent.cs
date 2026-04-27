using Shoply.WebApi.Common.Shared.Models;

namespace Shoply.WebApi.Features.WebHooks.Models;

[ValueObject<Guid>]
public readonly partial struct WebHookEventId;

public enum WebHookEventType
{
    Stripe
}

public enum WebHookEventStatus
{
    Pending,
    Success,
    Failed
}

public sealed class WebHookEvent : Entity<WebHookEventId>
{
    private WebHookEvent() { } 
    
    public WebHookEvent(WebHookEventType eventType, string eventId, string payload)
    {
        Id = WebHookEventId.From(Guid.CreateVersion7());
        EventType = eventType;
        EventId = eventId;
        Payload = payload;
        RetryCount = 0;
        Status = WebHookEventStatus.Pending;
    }
    
    public WebHookEventType EventType { get; init; }
    public WebHookEventStatus Status { get; init; }
    public string EventId { get; init; }
    public string Payload { get; init; }
    public int RetryCount { get; private set; }
    
    public void IncrementRetryCount()
    {
        RetryCount++;
    }
}