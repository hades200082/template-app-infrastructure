# Proof of concept for docker-compose

This repo demonstrates a working [docker-compose.yml](./docker-compose.yml) that includes:

- [Azure Cosmos DB Emulator for Linux](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=docker-hub)
- [RabbitMQ](https://www.rabbitmq.com) ([Docker image](https://hub.docker.com/r/masstransit/rabbitmq) provided by [MassTransit.io](https://masstransit.io/quick-starts/rabbitmq))

The purpose of this is to provide an example of how to configure docker-compose for a local development environment that only requires the developer to run one command to have all required services running on their local machine in docker containers.

The aim of this is to demonstrate a working example.

## Requirements

- .Net 7 SDK
- Docker for Windows (or regular docker on Linux)

## Developer setup

### Services currently configured

- `cosmosdb`: Azure Cosmos DB Emulator for Linux
- `azurite`: Azurite (Azure storage emulator)
- `rabbitmq`: RabbitMQ AMQP services
- `azure-search`: Unofficial Azure Search emulator using Solr

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

This is the Azure Serverless Functions application. Its purpose is to communicate with the AMQP service as both sender
and receiver.

It will have a function to monitor the Cosmos DB change feed and send commands or publish events into the AMQP service.

It will also register consumers which will handle incoming events and commands from the AMQP service, 
writing to Cosmos DB where needed.

#### server/shared/Shared.MessageModels

This is a .Net Standard 2.0 Class Library project. .Net Standard was specifically chosen for its compatibility with the
.Net Framework 4.6.1 and up since we don't know what version of .Net 3rd parties might be using.

The idea of this project is that it may be deployed as a stand-alone nuget package so that 3rd parties that we are 
integrating with via the AMQP services can simple install the package and use exactly the same models as us.

Only POCO models should be located in this class library project.

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
