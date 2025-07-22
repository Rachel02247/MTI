namespace MultiTanentInvetory.Model.Entities
{
    public record InventoryItem
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Category { get; init; }
        public string Description { get; init; }
        public string TenantId { get; init; }
        public bool IsActive { get; init; }
        public bool IsCheckedOut { get; init; }
    }
    //(int Id, string Name, string Category, string TenantId, bool IsActive);

}
