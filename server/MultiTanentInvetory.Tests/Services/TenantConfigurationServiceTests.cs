using Microsoft.Extensions.Options;
using Moq;
using MultiTanentInvetory.Model.Config;
using MultiTanentInvetory.Services;
using Xunit;

namespace MultiTanentInvetory.Tests.Services;

public class TenantConfigurationServiceTests
{
    private readonly Mock<IOptionsMonitor<List<TenantSettings>>> _mockOptionsMonitor;
    private readonly TenantConfigurationService _service;

    public TenantConfigurationServiceTests()
    {
        _mockOptionsMonitor = new Mock<IOptionsMonitor<List<TenantSettings>>>();
        var mockOptions = new Mock<IOptions<List<TenantSettings>>>();
        _service = new TenantConfigurationService(mockOptions.Object, _mockOptionsMonitor.Object);
    }

    [Fact]
    public void GetSettingsForTenant_ReturnsCorrectSettings()
    {
        // Arrange
        var tenantId = "alpha-logistics";
        var tenantSettings = new List<TenantSettings>
        {
            new() 
            { 
                TenantName = tenantId,
                EnableCheckout = true,
                MaxItemsPerUser = 3,
                AllowedItemCategories = new List<string> { "Electronics" }
            }
        };

        _mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(tenantSettings);

        // Act
        var result = _service.GetSettingsForTenant(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenantId, result.TenantName);
        Assert.True(result.EnableCheckout);
        Assert.Equal(3, result.MaxItemsPerUser);
        Assert.Contains("Electronics", result.AllowedItemCategories);
    }

    [Fact]
    public void GetSettingsForTenant_WithInvalidTenant_ReturnsNull()
    {
        // Arrange
        var tenantSettings = new List<TenantSettings>
        {
            new() { TenantName = "alpha-logistics" }
        };

        _mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(tenantSettings);

        // Act
        var result = _service.GetSettingsForTenant("invalid-tenant");

        // Assert
        Assert.Null(result);
    }
}