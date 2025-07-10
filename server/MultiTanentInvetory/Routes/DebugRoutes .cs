

namespace MultiTanentInvetory.Routes;

public static class DebugRoutes
{
    public static IEndpointRouteBuilder MapDebugRoutes(this IEndpointRouteBuilder app)
    {


        app.MapGet("debug/data", (AppDbContext _context) => {
            var data = _context.InventoryItems.ToList();
            return Results.Ok(data);
        });




        return app;
    }

}