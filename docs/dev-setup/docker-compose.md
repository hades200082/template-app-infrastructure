# Running local services

## Docker compose 

This repo uses [docker-compose](./docker-compose.yml) including the following services:

- [Azure Cosmos DB Emulator for Linux](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=docker-hub)
- [RabbitMQ](https://www.rabbitmq.com) ([Docker image](https://hub.docker.com/r/masstransit/rabbitmq) provided by [MassTransit.io](https://masstransit.io/quick-starts/rabbitmq))
- [Azure Search Emulator](https://github.com/tomasloksa/azure-search-emulator) (currently disabled due to issues I need to investigate)

## Service names

- `cosmosdb`: Azure Cosmos DB Emulator for Linux
- `azurite`: Azurite (Azure storage emulator)
- `rabbitmq`: RabbitMQ AMQP services
- `azure-search`: Unofficial Azure Search emulator using Solr (currently disabled due to issues I need to investigate)

## Assumptions

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

## Running services

### The first time

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
