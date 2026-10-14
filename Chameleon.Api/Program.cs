using Chameleon.Api;
using Chameleon.CommonApiLib;
using Chameleon.CosmosData;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails(opts => // built-in problem details support
    opts.CustomizeProblemDetails = (ctx) =>
    {
        if (!ctx.ProblemDetails.Extensions.ContainsKey("traceId"))
        {
            string? traceId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
            ctx.ProblemDetails.Extensions.Add(new KeyValuePair<string, object?>("traceId", traceId));
        }
        var exception = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ctx.ProblemDetails.Status == 500)
        {
            ctx.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    }
);

// Add services to the container.

builder.AddKeyedAzureCosmosClient("cosmos-connection");
builder.Services.AddSingleton<IDataAccess>(sp =>
{
    var client = sp.GetRequiredKeyedService<CosmosClient>("cosmos-connection");
    return new CosmosDataAccess(
        client:client,
        databaseName: "chameleon-db",
        containerName: "chameleon-container",
        partitionKeyPath: "/project"
    );
});
builder.Services.AddSingleton<JsonParser>();
builder.Services.AddSingleton<XmlParser>();
builder.Services.AddSingleton<IProcessRequestActionLogic, ProcessRequestActionLogic>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<CosmosDataSeeder>();

// Build app
var app = builder.Build();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

await SetupSeedDataAsync(app);

app.MapControllers();
app.Run();

[ExcludeFromCodeCoverage]
static async Task SetupSeedDataAsync(WebApplication app)
{
    // only run this for the Development environment.
    if (!app.Environment.IsDevelopment()) return;

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dataSeeder = services.GetRequiredService<CosmosDataSeeder>();
        await dataSeeder.CreateAsync();
    }
}


public partial class Program { } // used for integration tests



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//app.UseCors("AllowSpecificOrigin");
// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/
//app.UseHttpsRedirection();
//app.UseAuthorization();


