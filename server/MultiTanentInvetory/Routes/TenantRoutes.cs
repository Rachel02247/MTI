
using MultiTanentInventory.Model.DTOs;

namespace MultiTanentInventory.Routes;

public static class TenantRoutes
{

    public static IEndpointRouteBuilder MapTenantRoutes(this IEndpointRouteBuilder app)
    {
        var inventoryGroup = app.MapGroup("/api/tenant");

        inventoryGroup.MapGet("", GetAll);
        inventoryGroup.MapPost("", Create);


        return app;
    }

    static async Task<Results<Ok<TenantDto[]>, UnauthorizedHttpResult>> GetAll(
        ITenantService tenantService)
    {
        var tenants = await tenantService.GetTenantsAsync();
        return TypedResults.Ok(tenants.ToArray());
    }

    static async Task<Results<Created<TenantDto>, UnauthorizedHttpResult>> Create(
        TenantDto tenant,
        ITenantService tenantService)
    {
        var newTenant = await tenantService.AddTenant(tenant);
        return TypedResults.Created($"/api/tenant/{newTenant.Id}", newTenant);
    }

}