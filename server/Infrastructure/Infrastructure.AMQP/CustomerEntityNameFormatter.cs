using MassTransit;

namespace Infrastructure.AMQP;

internal sealed class CustomerEntityNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>()
    {
        var formatter = new KebabCaseEndpointNameFormatter(true);
        return formatter.SanitizeName(typeof(T).Name);
    }
}
