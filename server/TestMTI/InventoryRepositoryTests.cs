using Microsoft.EntityFrameworkCore;
using MultiTanentInvetory.Data;
using MultiTanentInvetory.Model.Entities;
using MultiTanentInvetory.Services;
using Xunit;

namespace MultiTanentInvetory.Tests.Services;

public class InventoryRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly InventoryRepository _repository;

    public InventoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new InventoryRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyTenantItems()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        await _context.InventoryItems.AddRangeAsync(new[]
        {
            new InventoryItem { Name = "Item1", TenantId = tenantId },
            new InventoryItem { Name = "Item2", TenantId = "other-tenant" }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(tenantId);

        // Assert
        Assert.Single(result);
        Assert.Equal("Item1", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectItem()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var item = new InventoryItem { Name = "Item1", TenantId = tenantId };
        await _context.InventoryItems.AddAsync(item);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(item.Id, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Item1", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesItemCorrectly()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var item = new InventoryItem { Name = "Item1", TenantId = tenantId };
        await _context.InventoryItems.AddAsync(item);
        await _context.SaveChangesAsync();

        var updatedItem = item with { IsCheckedOut = true };

        // Act
        await _repository.UpdateAsync(updatedItem);

        // Assert
        var result = await _context.InventoryItems.FindAsync(item.Id);
        Assert.True(result?.IsCheckedOut);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}