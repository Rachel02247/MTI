namespace Constracs;
public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetAllAsync(string tenantId);
    Task<InventoryItem?> GetByIdAsync(int id, string tenantId);
    Task AddAsync(InventoryItem item);
    Task UpdateAsync(InventoryItem item);
}
