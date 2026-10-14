var builder = DistributedApplication.CreateBuilder(args);

var cosmosInst = builder.AddAzureCosmosDB("cosmos-connection").RunAsEmulator(emulator =>
    emulator
        .WithDataExplorer()
    );
var cosmosDb = cosmosInst.AddCosmosDatabase("chameleon-database", "chameleon-db");
var cosmosContainer = cosmosDb.AddContainer("chameleon-container", "/project");

builder.AddProject<Projects.Chameleon_Api>("chameleon-api")
    .WithExternalHttpEndpoints()
    .WaitFor(cosmosContainer)
    .WithReference(cosmosInst);

builder.Build().Run();
