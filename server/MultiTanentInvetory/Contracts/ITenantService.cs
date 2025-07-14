
namespace MultiTanentInventory.Contracts;

public interface ITenantService
    {
        Task<IEnumerable<TenantDto>> GetTenantsAsync();
    Task<TenantDto> AddTenant(TenantDto tenant);
}

