

namespace MultiTanentInvetory.Extensions;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddApplicationConfiguration(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();


        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
        });

        builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", policy =>
          policy.WithOrigins("http://localhost:5020", "http://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()

        ));


        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging();
        });


        builder.Services.Configure<List<TenantSettings>>(builder.Configuration.GetSection("Tenants"));

        builder.Services.AddSignalR();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAutoMapper(typeof(MappingProfile));


        builder.Services.AddScoped<ITenantContext, TenantContext>();
        builder.Services.AddScoped<ITenantService, TenantService>();
        builder.Services.AddScoped<ITenantRepository, TenantRepository>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddSingleton<ITenantConfigurationService, TenantConfigurationService>();
        builder.Services.AddScoped<IClientNotifier, SignalRClientNotifier>();
        builder.Services.AddScoped<ISignalRCallerContext, SignalRCallerContext>();



        return builder;
    }
}
