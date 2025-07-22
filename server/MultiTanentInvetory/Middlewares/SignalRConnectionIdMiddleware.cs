
using Microsoft.Extensions.Primitives;

namespace MultiTanentInvetory.Middlewares;


public class SignalRConnectionIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SignalRConnectionIdMiddleware> _logger;

    public SignalRConnectionIdMiddleware(
        RequestDelegate next,
        ILogger<SignalRConnectionIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ISignalRCallerContext callerContext)
    {
        _logger.LogDebug("SignalRConnectionIdMiddleware invoked for path: {Path}", context.Request.Path);

        try
        {
            if (context.Request.Headers.TryGetValue("X-SignalR-Connection-ID", out var connectionId) &&
                  !StringValues.IsNullOrEmpty(connectionId) &&
                  !string.IsNullOrWhiteSpace(connectionId))
            {
                callerContext.CurrentConnectionId = connectionId;
                _logger.LogInformation("SignalR connectionId set from header: {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogDebug("Header X-SignalR-Connection-ID not present or empty – continuing without update.");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading x-signalr-connection-id header");
        }

        await _next(context);
    }
}


