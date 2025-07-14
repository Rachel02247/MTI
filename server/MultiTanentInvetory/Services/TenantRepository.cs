

namespace MultiTanentInventory.Services;

public class TenantRepository(AppDbContext _context) : ITenantRepository
{
    
   
    public async Task<List<Tenant>> GetTenantsAsync()
    {
        return await _context.Tenants.ToListAsync();
    }

    public async Task AddTenantAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
    }
}

