
namespace MultiTanentInvetory.Services;

public class InventoryService(
    IInventoryRepository _inventoryRepository,
    ITenantConfigurationService _tenantConfigurationService,
    IClientNotifier _clientNotifier,
    IMapper _mapper) : IInventoryService
{
    public async Task<IEnumerable<InventoryItemDto>> GetAllAsync(string tenantId)
    {
        var items = await _inventoryRepository.GetAllAsync(tenantId);

        return items
            .Where(i => i.IsActive)
            .Select(i => _mapper.Map<InventoryItemDto>(i, opt => opt.Items["TenantId"] = tenantId));
    }

    public async Task<InventoryItemDto?> GetByIdAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;

        return _mapper.Map<InventoryItemDto>(item, opt => opt.Items["TenantId"] = tenantId);
    }

    public async Task<InventoryItemDto> CreateAsync(CreateOrUpdateItemRequest request, string tenantId)
    {
        var newItem = _mapper.Map<InventoryItem>(request);
        newItem = newItem with
        {
            TenantId = tenantId,
            IsActive = true,
            IsCheckedOut = false
        };

        await _inventoryRepository.AddAsync(newItem);

        var dto = _mapper.Map<InventoryItemDto>(newItem, opt => opt.Items["TenantId"] = tenantId);
        await _clientNotifier.NotifyItemAdded(dto);
        return dto;
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

        var dto = _mapper.Map<InventoryItemDto>(item, opt => opt.Items["TenantId"] = tenantId);
        await _clientNotifier.NotifyItemUpdated(dto);
        return dto;
    }

    public async Task<bool> SoftDeleteAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item is null) return false;

        item = item with { IsActive = false };
        await _inventoryRepository.UpdateAsync(item);

        var dto = _mapper.Map<InventoryItemDto>(item, opt => opt.Items["TenantId"] = tenantId);
        await _clientNotifier.NotifyItemRemoved(dto);
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

        await _clientNotifier.NotifyItemCheckOut(item.Id, item.TenantId);

        return null;
    }

    public async Task<string?> CheckInAsync(int id, string tenantId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id, tenantId);
        if (item == null || !item.IsActive) return null;

        if (!item.IsCheckedOut) return "Item not checked out";

        item = item with { IsCheckedOut = false };
        await _inventoryRepository.UpdateAsync(item);

        await _clientNotifier.NotifyItemCheckIn(id, item.TenantId);

        return null;
    }

    public async Task<bool> CanCompanyCheckoutMoreAsync(string tenantId)
    {
        var config = _tenantConfigurationService.GetSettingsForTenant(tenantId);
        if (config == null)
            return false;

        var allItems = await _inventoryRepository.GetAllAsync(tenantId);
        var checkedOutCount = allItems.Count(i => i.IsCheckedOut);

        return checkedOutCount < config.MaxItemsPerUser;
    }
}
