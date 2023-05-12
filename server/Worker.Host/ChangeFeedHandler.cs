using Application.Mappers;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Cosmos;
using MassTransit;
using Microsoft.Azure.Cosmos;

namespace Worker.Host;

internal sealed class ChangeFeedHandler : IChangeFeedHandler
{
    private readonly IBus _bus;

    public ChangeFeedHandler(
        IBus bus
    )
    {
        _bus = bus;
    }

    public async Task HandleChangeFeedAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<Entity> changes, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        foreach (var entity in changes)
        {
            /*
             * Use a switch statement to respond to changes based on entity type
             */
            switch (entity)
            {
                case ExampleEntity exampleEntity:
                    tasks.Add(HandleMyEntityAsync(exampleEntity, cancellationToken));
                    break;
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async Task HandleMyEntityAsync(ExampleEntity ent, CancellationToken cancellationToken = default)
    {
        var mapper = new ExampleEntityMapper();
        await _bus.Publish(mapper.EntityToMessage(ent), cancellationToken);
    }
}
