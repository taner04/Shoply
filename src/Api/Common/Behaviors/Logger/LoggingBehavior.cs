using System.Diagnostics;

namespace Api.Common.Behaviors.Logger;

public sealed partial class LoggingBehavior<TMessage, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    ILogger<LoggingBehavior<TMessage, TResponse>> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly string _behaviorType = typeof(TMessage).FullName ?? typeof(TMessage).Name;
    private readonly string _messageName = typeof(TMessage).Name;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var ctx = LoggerRequestContext.FromHttpContext(httpContextAccessor.HttpContext);

            LogBeginHandling(
                _messageName,
                _behaviorType,
                ctx.ToString()
            );

            return await next(message, cancellationToken);
        }
        catch (Exception e)
        {
            LogOccuredError(_messageName, e);
            throw;
        }
        finally
        {
            sw.Stop();
            LogFinishedHandling(_messageName, sw.ElapsedMilliseconds);
        }
    }

    [LoggerMessage(0, LogLevel.Information,
        "Beginning {messageName} ({behaviorType}) with context {messageContext}")]
    private partial void LogBeginHandling(
        string messageName,
        string behaviorType,
        string messageContext);

    [LoggerMessage(1, LogLevel.Error, "Error handling {messageName}")]
    private partial void LogOccuredError(
        string messageName,
        Exception exception);

    [LoggerMessage(2, LogLevel.Information,
        "Finished handling {messageName} in {elapsedMs} ms.")]
    private partial void LogFinishedHandling(
        string messageName,
        long elapsedMs);
}