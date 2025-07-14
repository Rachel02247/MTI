namespace MultiTanentInventory.Contracts;

public interface ITenantRepository
{
    Task<List<Tenant>> GetTenantsAsync();
    Task AddTenantAsync(Tenant tenant);
}
