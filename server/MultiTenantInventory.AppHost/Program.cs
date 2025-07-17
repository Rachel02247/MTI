using MultiTenantInventory.AppHost.Extentions;


var builder = DistributedApplication.CreateBuilder(args);

builder.AppHostResources();


builder.Build().Run();
