namespace MultiTanentInventory.Contracts
{
    public interface IClientNotifier
    {
        Task NotifyItemAdded(InventoryItemDto itemThatAdded);
        Task NotifyItemRemoved(InventoryItemDto itemThatRemoved);
        Task NotifyItemUpdated(InventoryItemDto itemThatUpdated);
        Task NotifyItemCheckOut(int itemId, string tenantId);
        Task NotifyItemCheckIn(int itemId, string tenantId);


    }
}
