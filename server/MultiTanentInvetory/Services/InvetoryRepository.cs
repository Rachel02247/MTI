namespace MultiTanentInvetory.Services;

public class InventoryRepository(AppDbContext _context) : IInventoryRepository
{
    public async Task<List<InventoryItem>> GetAllAsync(string tenantId)
    {
        return await _context.InventoryItems
            .Where(i => i.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<InventoryItem?> GetByIdAsync(int id, string tenantId)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == tenantId);
    }

    public async Task AddAsync(InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryItem item)
    {
        var trackedEntity = _context.InventoryItems.Local.FirstOrDefault(e => e.Id == item.Id);
        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id, string tenantId)
    {
        var item = await GetByIdAsync(id, tenantId);
        if (item is null)
            return;

        item = item with { IsActive = false };
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
