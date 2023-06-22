# Infrastructure.AMQP

The solution uses the [MassTransit](https://masstransit.io/) package to handle communications
via message queue services over the AMPQ protocol.

This can be configured in various ways to use different transports such as RabbitMQ, 
Azure Service Bus, Amazon SQS, etc.

## Configuration

This service is configured to automatically look for and use a RabbitMQ instance running locally in docker.

RabbitMQ is included in the solution's [docker compose](../dev-setup/docker-compose.md) setup.

In the `Infrastructure.AMQP.ServiceCollectionExtensions` class, `AddAmqp()` static method you
can configure other transports for the production and other envrinonments as needed in the `else`
section on line 30.

## Usage

### Where to use it

The AMQP services should only be used in the `Worker.Host` project or it's dependencies. 

The `API.Host` project should not talk to AMQP at all, instead relying on the Cosmos DB Change Feed
to handle triggering any messages or notifications that need to be sent.  

### How to use it

In classes that are registered as singletons you should inject and use the `IBus` interface to send messages/notifications.

In classes that are scoped in some way (transient, scoped, etc.) you should use the scope-aware, specialised 
interfaces instead:

- `ISendEndpointProvider` is used to retrieve a `SendEndpoint` for sending messages to queues
- `IPublishEndpoint` provides a single scoped endpoint for publishing notifications.
