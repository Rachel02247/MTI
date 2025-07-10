
namespace MultiTanentInvetory.Services
{
    public class TenantContext: ITenantContext
    {
        public string? CurrentTenantId { get; set; }

    }
}
