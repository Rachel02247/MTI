namespace MultiTanentInvetory.Model.Config;


public class TenantSettings
{
    public string TenantName { get; set; }
    public bool EnableCheckout { get; set; }
    public int MaxItemsPerUser { get; set; }
    public List<string> AllowedItemCategories { get; set; } = new();
}