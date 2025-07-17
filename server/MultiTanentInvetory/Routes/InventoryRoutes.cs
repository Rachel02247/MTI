
using Microsoft.AspNetCore.Http;

namespace MultiTanentInvetory.Routes;

public static class InventoryRoutes
{
    public static IEndpointRouteBuilder MapInventoryRoutes(this IEndpointRouteBuilder app)
    {
        var inventoryGroup = app.MapGroup("/api/items");

        inventoryGroup.MapGet("", GetAll);
        inventoryGroup.MapGet("{id}", GetById);
        inventoryGroup.MapPost("", Create);
        inventoryGroup.MapPut("{id}", Update);
        inventoryGroup.MapDelete("{id}", SoftDelete);
        inventoryGroup.MapPost("{id:int}/checkout", CheckOut);
        inventoryGroup.MapPost("{id:int}/checkin", CheckIn);

        return app;
    }

    public static async Task<Results<Ok<InventoryItemDto[]>, UnauthorizedHttpResult>> GetAll(
        IInventoryService inventoryService,
        ITenantContext tenantContext
        )
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();

        var result = await inventoryService.GetAllAsync(tenantId);
        return TypedResults.Ok(result.ToArray());
    }

    public static async Task<Results<Ok<InventoryItemDto>, NotFound, UnauthorizedHttpResult>> GetById(
        int id,
        IInventoryService inventoryService,
        ITenantContext tenantContext)
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();

        var item = await inventoryService.GetByIdAsync(id, tenantId);
        if (item == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(item);
    }

    public static async Task<Results<Created<InventoryItemDto>, UnauthorizedHttpResult>> Create(
        CreateOrUpdateItemRequest request,
        IInventoryService inventoryService,
        ITenantContext tenantContext
        )
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();


        var newItem = await inventoryService.CreateAsync(request, tenantId);
        return TypedResults.Created($"/api/items/{newItem.Id}", newItem);
    }

    public static async Task<Results<Ok<InventoryItemDto>, NotFound, UnauthorizedHttpResult>> Update(
        int id,
        CreateOrUpdateItemRequest request,
        IInventoryService inventoryService,
        ITenantContext tenantContext
        )
    {
        var tenantId = tenantContext.CurrentTenantId;

        if (tenantId == null)
            return TypedResults.Unauthorized();


        var updatedItem = await inventoryService.UpdateAsync(id, request, tenantId);
        if (updatedItem == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(updatedItem);
    }

    public static async Task<Results<Ok<string>, NotFound, UnauthorizedHttpResult>> SoftDelete(
        int id,
        IInventoryService inventoryService,
        ITenantContext tenantContext
        )
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();


        var deleted = await inventoryService.SoftDeleteAsync(id, tenantId);
        if (!deleted)
            return TypedResults.NotFound();

        return TypedResults.Ok("Item was soft deleted successfully.");
    }

    public static async Task<Results<Ok<string>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> CheckOut(
        int id,
        ITenantContext @object,
        IInventoryService inventoryService,
        ITenantContext tenantContext)
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();

        var result = await inventoryService.CheckOutAsync(id, tenantId);
        return result switch
        {
            null => TypedResults.Ok("Checkout successful"),
            "NotFound" => TypedResults.NotFound(),
            _ => TypedResults.BadRequest(result)
        };
    }

    public static async Task<Results<Ok<string>, NotFound, BadRequest<string>, UnauthorizedHttpResult>> CheckIn(
        int id,
        IInventoryService inventoryService,
        ITenantContext tenantContext)
    {
        var tenantId = tenantContext.CurrentTenantId;
        if (tenantId == null)
            return TypedResults.Unauthorized();

        var result = await inventoryService.CheckInAsync(id, tenantId);
        return result switch
        {
            null => TypedResults.Ok("Check-in successful"),
            "NotFound" => TypedResults.NotFound(),
            _ => TypedResults.BadRequest(result)
        };
    }
}
