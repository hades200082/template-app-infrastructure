# Template app infrastructure

This repo uses [docker-compose](./docker-compose.yml) including the following services:

- [Azure Cosmos DB Emulator for Linux](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=docker-hub)
- [RabbitMQ](https://www.rabbitmq.com) ([Docker image](https://hub.docker.com/r/masstransit/rabbitmq) provided by [MassTransit.io](https://masstransit.io/quick-starts/rabbitmq))
- [Azure Search Emulator](https://github.com/tomasloksa/azure-search-emulator) (currently disabled due to issues I need to investigate)

## Requirements

- .Net 7 SDK
- Docker for Windows (or regular docker on Linux)

## Developer setup

### Services currently configured

- `cosmosdb`: Azure Cosmos DB Emulator for Linux
- `azurite`: Azurite (Azure storage emulator)
- `rabbitmq`: RabbitMQ AMQP services
- `azure-search`: Unofficial Azure Search emulator using Solr (currently commented out)

### Assumptions

1. You have the requirements listed above installed
2. You have no other services running on any of the following ports:
    - 5672
    - 8000
    - 8081
    - 8900-8901
    - 8979
    - 8983
    - 10000-10002
    - 10250-10256
    - 10350
    - 15672
3. You do not already have Azurite running elsewhere.
4. You do not have the (now deprecated) Azure Storage Emulator running

> ***Note:** You may have an instance of Azurite or the older storage emulator running without realising it if you have
> opened another project that has a connection string `UseDevelopmentStorage=true`. Visual Studio will start its own
> instance automatically when it detects it is needed. Note that the Visual Studio Azurite/Emulator instance does not
> shut down when you close Visual Studio - you must restart your computer.*

### Project overviews

#### server/Api.Host

This is a RESTful API designed for the front-end application to use. Its primary purpose is to manage authentication
and to facilitate data access to Cosmos DB.

#### server/Functions.Host

This is the Azure Serverless Functions application. Its purpose is to communicate with the AMQP service as a sender.

It will have a function to monitor the Cosmos DB change feed and send commands or publish events into the AMQP service.

#### server/Worker.Host (WIP)

This is a .Net worker service that will act as the consumer & worker handling incoming commands or events from the AMQP service.

#### server/Shared/Shared.MessageModels

This is a .Net Standard 2.0 Class Library project. .Net Standard was specifically chosen for its compatibility with the
.Net Framework 4.6.1 and up since we don't know what version of .Net 3rd parties might be using.

The idea of this project is that it may be deployed as a stand-alone nuget package so that 3rd parties that we are
integrating with via the AMQP services can simple install the package and use exactly the same models as us.

Only POCO models should be located in this class library project.

#### server/Infrastructure/*

These projects are class libraries that each deal with setup and DI registration of a single external concern (i.e. cosmos DB)
such that it can be included in multiple *.Host projects as needed.

These projects may include wrapping services/repositories, etc. as needed but should strive to exclude any business logic.

#### server/Application/*

This is where the business logic lives.

#### server/Domain/*

This is where our entities live.

#### client/

This is the front-end application which is a NextJS application.

### Running services for the first time

1. Clone the repository
2. Open your command line client that can use docker CLI commands and `cd` to the repository root
3. Run the command `docker compose up -d`

This will start all of the services, all running on their default ports.

> ***Note:***
> *Before starting the services or at any point during development, you may need to amend the docker-compose.yml file to
> specify the Solr "cores" that you require. In Solr, cores are analogous to "indexes".*
>
> *See https://github.com/tomasloksa/azure-search-emulator/blob/master/README.md for more information on how to use*
> *this emulator, prepopulate indexes, etc.*
>
> When you change the cores in this way all developers working on the project must run `docker compose down` and re-up
> their local containers to receive the changes.

### Closing down services when finished

When you've finished using the services for the day it's a good idea to spin down the containers to free up system
resources.

1. Open your command line client that can use docker CLI commands and `cd` to the repository root
2. Run the command `docker compose down`

> ***Note:***
> *This will remove all containers for these services but will **not** remove any data from docker volumes*

### Restarting the services the next time

Use `docker compose up -d` to spin them up again as this will recreate the containers.

### Accessing the Services

- Cosmos Explorer: https://localhost:8081/_explorer/index.html
- RabbitMQ Manager: http://localhost:15672/ (u: `guest` | p: `guest`)
- Azurite: Use [Azure Storage Explorer](https://azure.microsoft.com/en-gb/products/storage/storage-explorer/) to connect to the local storage emulator.

### Running the solution

1. Ensure that your docker containers are running as described above.
2. Ensure that you have trusted the dotnet developer certificates by running the command `dotnet dev-certs https --trust`
3. Open the [api](./api) solution in visual Studio or Jetbrains Rider.
4. Run the "https" runtime configuration (it should be the only one available)

This will open Swagger in your default browser. There are two POST endpoints available:

#### /TestCosmos

This endpoint will create a `TestEntity` in your cosmosdb instance. You can verify this with the Cosmos Explorer.

#### /TestMassTransit

This endpoint will do two things:

1. **Publish a notification to RabbitMQ.**
   You can verify this by checking to see if an exchange exists named after the
   fully qualified type used for the message (in this case `Host.Models.TestMessage`). If you view the exchange
   immediately after running the endpoint you will see the spike in the graph showing the message being sent.
2. **Send a message to a queue in RabbitMQ.**
   You can verify this by locating a queue with the name "test" in the
   queues section. Within this queue you can use the management console to get the messages and see their data.
