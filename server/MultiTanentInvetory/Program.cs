
var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfiguration();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<TenantMiddleware>();

app.MapInventoryRoutes();
app.MapTenantRoutes();
app.MapDebugRoutes();

app.Run();