using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable CA1812
// warning disabled since ConfigureSwaggerOptions is detected by the framework, not direclty instantiated

namespace Api.Host;

internal sealed class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(
        ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Example API",
            Version = description.ApiVersion.ToString(),
            Description = "This is an example API. Be sure to update the API definition in API.Host/ConfigureSwaggerOptions.cs",
            Contact = new OpenApiContact{Name = "Template app infrastructure", Url = new Uri("https://github.com/hades200082/template-app-infrastructure")}
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
