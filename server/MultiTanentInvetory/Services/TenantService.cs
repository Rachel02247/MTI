
using MultiTanentInventory.Model.DTOs;

namespace MultiTanentInventory.Services;

public class TenantService(ITenantRepository _tenantRepository) : ITenantService
{
    
   
    public async Task<IEnumerable<TenantDto>> GetTenantsAsync()
    {
        var tenats = await _tenantRepository.GetTenantsAsync();
        return tenats.Select(t => new TenantDto(t.Id, t.Name));
       
    }

    public async Task<TenantDto> AddTenant(TenantDto tenant)
    {
        var newTenant = new Tenant(tenant.Id, tenant.Name);
        await _tenantRepository.AddTenantAsync(newTenant);

        return new TenantDto(newTenant.Id, newTenant.Name);

    }
}

