

using MultiTanentInventory.Services;

namespace MultiTanentInvetory.Extensions;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddApplicationConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
        });

        builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", policy =>
  policy.WithOrigins("http://localhost:5020", "http://localhost:4200")
  .AllowAnyHeader()
  .AllowAnyMethod()
));



        builder.Services.AddDbContext<AppDbContext>(options => {
            options.UseSqlite("Data Source=inventory.db");
            options.EnableSensitiveDataLogging();
        });


        builder.Services.Configure<List<TenantSettings>>(builder.Configuration.GetSection("Tenants"));

        builder.Services.AddScoped<ITenantContext, TenantContext>();
        builder.Services.AddScoped<ITenantService, TenantService>();
        builder.Services.AddScoped<ITenantRepository, TenantRepository>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddSingleton<ITenantConfigurationService, TenantConfigurationService>();



        return builder;
    }
}
