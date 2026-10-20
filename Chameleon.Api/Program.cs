using Chameleon.Api;
using Chameleon.CommonApiLib;
using Chameleon.CosmosData;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Chameleon.Parsers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails(opts =>
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
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<DatabaseDataSeeder>(sp =>
{
    var dataAccess = sp.GetRequiredService<IDataAccess>();
    var jsonParser = (MessageParserBase)sp.GetRequiredService<JsonParser>();
    var xmlParser = (MessageParserBase)sp.GetRequiredService<XmlParser>();
    return new DatabaseDataSeeder(
        dataAccess: dataAccess,
        jsonParser: jsonParser,
        xmlParser: xmlParser
    );     
});
 

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
        var dataSeeder = services.GetRequiredService<DatabaseDataSeeder>();
        await dataSeeder.CreateTestDataAsync();
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


