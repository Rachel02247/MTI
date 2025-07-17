

using Aspire.Hosting.Dashboard;
var builder = DistributedApplication.CreateBuilder(args);


var multiTanentInventory = builder.AddProject<Projects.MultiTanentInventory>("MultiTanentInventory");

var sqlPassword = builder.AddParameter("SqlServerPassword", secret: true);

builder.AddSqlServer("sqldb")
       .WithEnvironment("ACCEPT_EULA", "Y")
       .WithPassword(sqlPassword)
       .WithVolume("sqldata");




//builder.AddDashboard();

//multiTanentInventory.


builder.Build().Run();
