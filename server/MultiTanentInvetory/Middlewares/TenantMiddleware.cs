
namespace MultiTanentInvetory.Middlewares;


public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {

        if (context.Request.Path.StartsWithSegments("/api/tenant"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing X-Tenant-ID header.");
            return;
        }

        var tenantId = tenantHeader.ToString().Trim();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Empty X-Tenant-ID header.");
            return;
        }

        tenantContext.CurrentTenantId = tenantId;
        _logger.LogInformation("Tenant resolved: {TenantId}", tenantId);

        await _next(context);
    }
}
