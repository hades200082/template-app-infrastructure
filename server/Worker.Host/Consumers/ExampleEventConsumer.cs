using Application.MessageModels.Events;
using MassTransit;
using Shared.Core;

namespace Worker.Host.Consumers;

internal sealed class ExampleEventConsumer : IConsumer<ExampleEntityCreated>
{
    private readonly ILogger<ExampleEventConsumer> _logger;

    public ExampleEventConsumer(ILogger<ExampleEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ExampleEntityCreated> context)
    {
        await Task.Yield(); // remove this
        _logger.LogMethodCall(new {context});
        
        // Do work here
    }
}
