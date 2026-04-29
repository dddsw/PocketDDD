var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("PocketDDDContext");

var sessionize = builder.AddWireMock("sessionize")
    .WithMappingsPath("Mocks")
    .WithReadStaticMappings()
    .WithWatchStaticMappings();

var migrations = builder.AddProject<Projects.PocketDDD_Server_MigrationService>("migrations")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.PocketDDD_Server_WebAPI>("webapi")
    .WithEndpoint("https", e => { e.Port = 7081; e.IsProxied = false; })
    .WithReference(db)
    .WaitForCompletion(migrations)
    .WithReference(sessionize)
    .WaitFor(sessionize)
    .WithEnvironment("Sessionize__BaseAddress", sessionize.GetEndpoint("http"));

builder.AddProject<Projects.PocketDDD_BlazorClient>("blazorclient")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
