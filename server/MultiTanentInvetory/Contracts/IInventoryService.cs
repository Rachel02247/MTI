namespace MultiTanentInvetory.Contracts
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItemDto>> GetAllAsync(string tenantId);
        Task<InventoryItemDto?> GetByIdAsync(int id, string tenantId);
        Task<InventoryItemDto> CreateAsync(CreateInventoryItemRequest request, string tenantId);
        Task<bool> SoftDeleteAsync(int id, string tenantId);
        Task<string?> CheckOutAsync(int id, string tenantId);
        Task<string?> CheckInAsync(int id, string tenantId);
        Task<bool> CanCompanyCheckoutMoreAsync(string tenantId);

    }
}
