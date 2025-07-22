

var builder = WebApplication.CreateBuilder(args);


builder.AddApplicationConfiguration();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<SignalRConnectionIdMiddleware>();


app.MapHub<InventoryHub>("api/hubs/inventory");

app.MapDefaultEndpoints();

app.MapInventoryRoutes();
app.MapTenantRoutes();
app.MapDebugRoutes();

app.Run();