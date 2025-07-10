public record InventoryItemDto(
    int Id,
    string Name,
    string Category,
    bool IsActive,
    bool IsCheckedOut
);
