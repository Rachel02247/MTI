namespace MultiTanentInvetory.Services;
public class TenantConfigurationService : ITenantConfigurationService
{
    private readonly IOptionsMonitor<List<TenantSettings>> _optionsMonitor;

    public TenantConfigurationService(
        IOptions<List<TenantSettings>> options,
        IOptionsMonitor<List<TenantSettings>> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public TenantSettings? GetSettingsForTenant(string tenantId)
    {
        var currentSettings = _optionsMonitor.CurrentValue;
        return currentSettings.FirstOrDefault(t => t.TenantName == tenantId);
    }
}   