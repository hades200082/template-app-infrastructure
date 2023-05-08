using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.ReDoc;

#pragma warning disable CA1812
// warning disabled since ConfigureReDocOptions is detected by the framework, not direclty instantiated

namespace Api.Host;

internal sealed class ConfigureReDocOptions : IConfigureNamedOptions<ReDocOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureReDocOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(ReDocOptions options)
    {
        var latest = _provider.ApiVersionDescriptions
            .First(x => !x.IsDeprecated);

        options.DocumentTitle = latest.GroupName;
        options.SpecUrl = $"/swagger/v{latest.ApiVersion.MajorVersion}/swagger.json";
    }

    public void Configure(string? name, ReDocOptions options)
    {
        Configure(options);
    }
}
