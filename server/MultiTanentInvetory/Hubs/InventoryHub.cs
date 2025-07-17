
namespace MultiTanentInventory.Hubs;



public class InventoryHub(ISignalRCallerContext _callerContext) : Hub
{

    public async Task RegisterTenant(string tenantId)
    {
        string connectionId = Context.ConnectionId;

        _callerContext.CurrentConnectionId = connectionId;

        await Groups.AddToGroupAsync(connectionId, tenantId);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}


