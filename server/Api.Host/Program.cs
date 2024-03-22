using Api.Host;
using Domain.DataSeeds; // I don't like accessing the domain here but seed data belongs to the domain.
using HealthChecks.UI.Client;
using Infrastructure.AMQP;
using Infrastructure.Cosmos;
using Infrastructure.Identity;
using Infrastructure.Identity.Auth0;
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
        opt.SetMinimumSecondsBetweenFailureNotifications(60 * 60 * 2);
        opt.SetHeaderText("My Awesome Healthchecks");
        opt.AddHealthCheckEndpoint("app", "/_health");
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
builder.Services.AddCosmos(builder.Configuration.GetSection(CosmosOptions.ConfigurationSectionName), builder.Environment);
builder.Services.AddAuth0(builder.Configuration.GetSection(Auth0Options.ConfigurationSectionName)); // Adds Auth0 authentication with JWT
// builder.Services.AddAuth0AuthenticationApiClient(builder.Configuration.GetSection(Auth0Options.ConfigurationSectionName)); // Adds the Auth0 AuthenticationApiClient
// builder.Services.AddAuth0ManagementApiClient(builder.Configuration.GetSection(Auth0ManagementApiOptions.ConfigurationSectionName)); // Adds the Auth0 ManagementApiClient (including taking care of getting/refreshing a token automatically)
builder.Services.AddStorage(builder.Configuration.GetSection("StorageOptions"), builder.Environment);
builder.Services.AddMediator();
builder.Services.AddValidation();

builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy(b =>
    {
        var domains = builder.Configuration.GetSection("CorsDomains").Get<string[]>() ?? Array.Empty<string>();

        b.WithOrigins(domains)
            .WithMethods("DELETE", "GET", "PATCH", "POST", "PUT")
            .AllowAnyHeader();
    });
});

// Add all seeds in all loaded assemblies
builder.Services.AddDataSeeds();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Execute all seeds before registering middleware
await app.ExecuteDataSeedingAsync(app.Lifetime.ApplicationStopping).ConfigureAwait(false);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseReDoc();
app.UseHttpsRedirection();
app.UseCustomAuthentication();
app.MapControllers();
app.MapHealthChecks("/_health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
    options.ApiPath = "/healthchecks-api";
    options.UseRelativeApiPath = false;
    options.UseRelativeResourcesPath = false;
});
app.UseCors();



#pragma warning disable CA1031
try
{
    await app.RunAsync().ConfigureAwait(true);
}
catch (Exception ex)
{
#pragma warning disable CA1848
    logger.LogCritical(ex, "Application threw an unhandled exception and shut down");
#pragma warning restore CA1848
}
#pragma warning restore CA1031
