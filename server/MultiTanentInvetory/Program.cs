
using MultiTanentInventory.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<TenantMiddleware>();

app.MapHub<InventoryHub>("api/hubs/inventory");

app.MapInventoryRoutes();
app.MapTenantRoutes();
app.MapDebugRoutes();

app.Run();