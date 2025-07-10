namespace MultiTanentInvetory.Contracts;

public interface ITenantConfigurationService
{
    TenantSettings? GetSettingsForTenant(string tenantId);

}
