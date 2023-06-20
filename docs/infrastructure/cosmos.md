# Infrastructure.Cosmos

## Configuration

The CosmosDB integration is pre-configured to automatically connect to the local Cosmos DB emulator running in Docker.

Use docker compose to spin up the required services.

For non-local environments, configure appSettings as follows:

```json
{
  "CosmosOptions": {
    "AccountEndpoint": "", // Get this from your Cosmos instance
    "AccountKey": "",
    "DatabaseId": "",
    "ContainerName": ""
  }
}
```

## Usage

### Application setup

In `Program.cs` add the Cosmos services to DI:

```csharp
builder.Services.AddCosmos(builder.Configuration, builder.Environment);
```

Also add the healthcheck:

```csharp
builder.Services.AddHealthChecks()
    /*snip*/
    .AddCosmos()
    /*snip*/
    ;
```

### Creating new entities

All entities must implement `Domain.Abstractions.IEntity`.

You can inherit the abstract base class `Domain.Abstractions.Entity` for most standard entities 
or extend it further as needed.

### Accessing Cosmos data

To access data in Cosmos you must use the `IRepository<IEntity>` generic interface.

This can be injected from the DI container directly into your classes.

```csharp
internal class MyClass
{
    public MyClass(IRepository<MyEnitity> repository)
    {
        
    }
}
```

The generic repository has been set up in such a way as to ensure optimal performance. Try to use methods that are "in partition"

## The change feed

The Cosmos DB change feed is a way to be notified of changes happening in your containers 
and take action based on those changes. This is ideal for an event or message queue driven
system.

### Registering a change feed handler

To configure a change feed handler you must have a container in your CosmosDB instance called "leases".
You may need to create this manually in non-local environments.

The "leases" container is where the change feed processor will store information that allows it to be scaled without duplicating
its processing.

To register a change feed processor you must first create a class that implements `IChangeFeedHandler` and add
it to your DI container in `Program.cs`.

```csharp
services.AddSingleton<IChangeFeedHandler, MyChangeFeedHandler>();
```

Next, after the `.Build()` in `Program.cs` you must start the change feed within your hosted application.

```csharp
var changeFeedProcessor = await host.Services
    .GetRequiredService<IChangeFeedProvider>()
    .StartChangeFeedProcessorAsync<Entity>("Worker");
```

Finally, after the `host.Run()` command in your `Program.cs` you must close the change feed down gracefully.

```csharp
await changeFeedProcessor.StopAsync();
```