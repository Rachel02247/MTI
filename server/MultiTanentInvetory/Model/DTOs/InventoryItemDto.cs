public record InventoryItemDto(
    int Id,
    string Name,
    string Category,
    string Description,
    bool IsActive,
    bool IsCheckedOut
);
