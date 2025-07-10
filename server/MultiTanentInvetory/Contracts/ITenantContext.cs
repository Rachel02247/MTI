namespace MultiTanentInvetory.Contracts
{
    public interface ITenantContext
    {
        string? CurrentTenantId { get; set; }

    }
}
