using Microsoft.Extensions.Logging;
using Moq;
using MultiTanentInvetory.Model.Config;
using MultiTanentInvetory.Model.DTOs;
using MultiTanentInvetory.Model.Entities;
using MultiTanentInvetory.Routes;
using MultiTanentInvetory.Services;
using Xunit;

namespace MultiTanentInvetory.Tests.Routes;

public class InventoryRoutesTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Mock<ITenantConfigurationService> _mockConfigService;
    private readonly Mock<ILogger<InventoryItemDto>> _mockLogger;

    public InventoryRoutesTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockConfigService = new Mock<ITenantConfigurationService>();
        _mockLogger = new Mock<ILogger<InventoryItemDto>>();
    }

    [Fact]
    public async Task GetAll_WithValidTenant_ReturnsItems()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var items = new List<InventoryItem> 
        {
            new() { Id = 1, Name = "Item1", Category = "Electronics", TenantId = tenantId, IsActive = true }
        };

        _mockTenantContext.Setup(x => x.CurrentTenantId).Returns(tenantId);
        _mockRepo.Setup(x => x.GetAllAsync(tenantId)).ReturnsAsync(items);

        // Act
        var result = await InventoryRoutes.GetAll(_mockLogger.Object, _mockRepo.Object, _mockTenantContext.Object);

        // Assert
        var okResult = Assert.IsType<Ok<InventoryItemDto[]>>(result.Result);
        Assert.Single(okResult.Value);
        Assert.Equal("Item1", okResult.Value[0].Name);
    }

    [Fact]
    public async Task GetById_WithValidItem_ReturnsItem()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Item1", 
            Category = "Electronics", 
            TenantId = tenantId, 
            IsActive = true 
        };

        _mockTenantContext.Setup(x => x.CurrentTenantId).Returns(tenantId);
        _mockRepo.Setup(x => x.GetByIdAsync(1, tenantId)).ReturnsAsync(item);

        // Act
        var result = await InventoryRoutes.GetById(1, _mockLogger.Object, _mockRepo.Object, _mockTenantContext.Object);

        // Assert
        var okResult = Assert.IsType<Ok<InventoryItemDto>>(result.Result);
        Assert.Equal("Item1", okResult.Value.Name);
    }

    [Fact]
    public async Task CheckOut_WithValidItem_SuccessfullyChecksOut()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Item1", 
            Category = "Electronics", 
            TenantId = tenantId, 
            IsActive = true,
            IsCheckedOut = false
        };

        var settings = new TenantSettings 
        { 
            TenantName = tenantId,
            EnableCheckout = true,
            AllowedItemCategories = new List<string> { "Electronics" }
        };

        _mockTenantContext.Setup(x => x.CurrentTenantId).Returns(tenantId);
        _mockConfigService.Setup(x => x.GetSettingsForTenant(tenantId)).Returns(settings);
        _mockRepo.Setup(x => x.GetByIdAsync(1, tenantId)).ReturnsAsync(item);

        // Act
        var result = await InventoryRoutes.CheckOut(1, _mockTenantContext.Object, _mockConfigService.Object, _mockRepo.Object);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(result.Result);
        Assert.Equal("Checkout successful", okResult.Value);
    }
}