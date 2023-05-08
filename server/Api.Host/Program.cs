using Api.Host;
using Infrastructure.Cosmos;
using Infrastructure.Logging;
using Infrastructure.Storage;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
