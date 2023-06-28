using Api.Host;
using Domain.DataSeeds; // I don't like accessing the domain here but seed data belongs to the domain.
using HealthChecks.UI.Client;
using Infrastructure.AMQP;
using Infrastructure.Cosmos;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Storage;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

builder.Services.AddHealthChecks()
    .AddCosmos()
    .AddAMQP(builder.Environment)
    .AddStorage();

builder.Services.AddHealthChecksUI(opt => {
        opt.SetEvaluationTimeInSeconds(30);
        opt.MaximumHistoryEntriesPerEndpoint(60);
        opt.AddHealthCheckEndpoint("app", "/_health");
        // opt.AddHealthCheckEndpoint(name: "app", uri: "~/_health");
    })
    .AddInMemoryStorage();

// Add services to the container.
builder.Services.AddControllers();

#region Configure Swagger Services

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = ApiVersion.Parse("1");
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

#endregion

// Custom layers
builder.Services.AddCosmos(builder.Configuration, builder.Environment);
builder.Services.AddStorage(builder.Configuration, builder.Environment);
builder.Services.AddMediator();
builder.Services.AddValidation();
builder.Services.AddIdentity(builder.Configuration);

// Add all seeds in all loaded assemblies
builder.Services.AddDataSeeds();

var app = builder.Build();

// Execute all seeds before registering middleware
await app.ExecuteDataSeedingAsync(app.Lifetime.ApplicationStopping).ConfigureAwait(false);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseReDoc();
app.UseHttpsRedirection();
app.UseIdentity();
app.MapControllers();
app.UseHealthChecks("/_health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});
app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
    options.ApiPath = "/healthchecks-api";
    options.UseRelativeApiPath = false;
    options.UseRelativeResourcesPath = false;
});

await app.RunAsync().ConfigureAwait(true);
