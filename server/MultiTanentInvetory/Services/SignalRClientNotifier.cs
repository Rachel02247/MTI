
namespace MultiTanentInventory.Services;

public class SignalRClientNotifier(
    IHubContext<InventoryHub> _hubContext,
    ISignalRCallerContext _callerContext,
    ILogger<SignalRClientNotifier> _logger
) : IClientNotifier
{
    public async Task NotifyItemAdded(InventoryItemDto itemThatAdded)
    {
        var callerConnectionId = _callerContext.CurrentConnectionId;

        _logger.LogInformation("Notifying other clients about item added. TenantId: {TenantId}, ItemId: {ItemId}, Skipping ConnectionId: {ConnectionId}",
            itemThatAdded.TenantId, itemThatAdded.Id, callerConnectionId);

        await _hubContext.Clients
            .GroupExcept(itemThatAdded.TenantId, callerConnectionId ?? "")
            .SendAsync("itemAdded", itemThatAdded);
    }

    public async Task NotifyItemRemoved(InventoryItemDto itemThatRemoved)
    {
        var callerConnectionId = _callerContext.CurrentConnectionId;

        _logger.LogInformation("Notifying other clients about item removal. TenantId: {TenantId}, ItemId: {ItemId}, Skipping ConnectionId: {ConnectionId}",
            itemThatRemoved.TenantId, itemThatRemoved.Id, callerConnectionId);

        await _hubContext.Clients
            .GroupExcept(itemThatRemoved.TenantId, callerConnectionId ?? "")
            .SendAsync("itemDeleted", itemThatRemoved);
    }

    public async Task NotifyItemUpdated(InventoryItemDto itemThatUpdated)
    {
        var callerConnectionId = _callerContext.CurrentConnectionId;

        _logger.LogInformation("Notifying other clients about item update. TenantId: {TenantId}, ItemId: {ItemId}, Skipping ConnectionId: {ConnectionId}",
            itemThatUpdated.TenantId, itemThatUpdated.Id, callerConnectionId);

        await _hubContext.Clients
            .GroupExcept(itemThatUpdated.TenantId, callerConnectionId ?? "")    
            .SendAsync("itemUpdated", itemThatUpdated);
    }

    public async Task NotifyItemCheckOut(int itemId, string tenantId)
    {
        var callerConnectionId = _callerContext.CurrentConnectionId;

        _logger.LogInformation("Notifying other clients about item checkout. TenantId: {TenantId}, ItemId: {ItemId}, Skipping ConnectionId: {ConnectionId}",
            tenantId, itemId, callerConnectionId);

        await _hubContext.Clients
            .GroupExcept(tenantId, callerConnectionId ?? "")
            .SendAsync("itemCheckOut", itemId, tenantId);
    }

    public async Task NotifyItemCheckIn(int itemId, string tenantId)
    {
        var callerConnectionId = _callerContext.CurrentConnectionId;

        _logger.LogInformation("Notifying other clients about item checkin. TenantId: {TenantId}, ItemId: {ItemId}, Skipping ConnectionId: {ConnectionId}",
            tenantId, itemId, callerConnectionId);

        await _hubContext.Clients
            .GroupExcept(tenantId, callerConnectionId ?? "")
            .SendAsync("itemCheckIn",  itemId, tenantId);
    }
}
