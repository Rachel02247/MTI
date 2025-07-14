namespace MultiTanentInvetory.Services;

public class InventoryService(IInventoryRepository _inventoryRepository, ITenantConfigurationService _tenantConfigurationService) : IInventoryService
{
  
    public async Task<bool> CanCompanyCheckoutMoreAsync(string tenantId)
    {
        var config = _tenantConfigurationService.GetSettingsForTenant(tenantId);
        if (config == null)
            return false;

        var allItems = await _inventoryRepository.GetAllAsync(tenantId);
        var checkedOutCount = allItems.Count(i => i.IsCheckedOut);

        return checkedOutCount < config.MaxItemsPerUser;
    }

    public async Task<IEnumerable<InventoryItemDto>> GetAllAsync(string tenantId)
    {
        var items = await _inventoryRepository.GetAllAsync(tenantId);

        return items
            .Where(i => i.IsActive)
            .Select(i => new InventoryItemDto(i.Id, i.Name, i.Category, i.Description, i.IsActive, i.IsCheckedOut));
    }

    public async Task<InventoryItemDto?> GetByIdAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;

        return new InventoryItemDto(item.Id, item.Name, item.Category, item.Description, item.IsActive, item.IsCheckedOut);
    }

    public async Task<InventoryItemDto> CreateAsync(CreateOrUpdateItemRequest request, string tenantId)
    {
        var newItem = new InventoryItem
        {
            Name = request.Name,
            Category = request.Category,
            Description = request.Description,
            TenantId = tenantId,
            IsActive = true,
            IsCheckedOut = false
        };

        await _inventoryRepository.AddAsync(newItem);

        return new InventoryItemDto(newItem.Id, newItem.Name, newItem.Category, newItem.Description, newItem.IsActive, newItem.IsCheckedOut);
    }

    public async Task<InventoryItemDto?> UpdateAsync(int id, CreateOrUpdateItemRequest request, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;
        item = item with
        {
            Name = request.Name ?? item.Name,
            Category = request.Category ?? item.Category,
            Description = request.Description 
        };
        await _inventoryRepository.UpdateAsync(item);
        return new InventoryItemDto(item.Id, item.Name, item.Category, item.Description, item.IsActive, item.IsCheckedOut);
    }

    public async Task<bool> SoftDeleteAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item is null) return false;

        item = item with { IsActive = false };
        await _inventoryRepository.UpdateAsync(item);
        return true;
    }

    public async Task<string?> CheckOutAsync(int id, string tenantId)
    {
        var settings = _tenantConfigurationService.GetSettingsForTenant(tenantId);
        if (settings == null || !settings.EnableCheckout)
            return "Checkout not allowed";

        if (!await CanCompanyCheckoutMoreAsync(tenantId))
            return $"Maximum checked out items ({settings.MaxItemsPerUser}) reached";

        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;

        if (item.IsCheckedOut) return "Already checked out";
        if (!settings.AllowedItemCategories.Contains(item.Category))
            return $"Category '{item.Category}' not allowed";

        item = item with { IsCheckedOut = true };
        await _inventoryRepository.UpdateAsync(item);
        return null; 
    }

    public async Task<string?> CheckInAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;

        if (!item.IsCheckedOut) return "Item not checked out";

        item = item with { IsCheckedOut = false };
        await _inventoryRepository.UpdateAsync(item);
        return null; 
    }

}
