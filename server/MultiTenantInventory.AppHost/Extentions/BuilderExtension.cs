
namespace MultiTenantInventory.AppHost.Extentions;

public static class BuilderExtension
{
    public static void AppHostResources(this IDistributedApplicationBuilder builder)
    {

        var sql = builder.AddSqlServer("sql")
                 .AddDatabase("mtidb");


        var mtiApi = builder.AddProject<Projects.MultiTanentInventory>("mtiapi")
                         .WaitFor(sql)
                         .WithReference(sql);

        var mtiAngular = builder.AddNpmApp("mticlient", "./../../client/MTI-app", "start")
                             .WithEnvironment("NG_CLI_ANALYTICS", "false")
                             .WaitFor(mtiApi);

    }
}

