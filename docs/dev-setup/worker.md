# Running the worker

The Worker.Host project is designed for use with the CosmosDB change feed 
and to handle incoming messages from the AMQP message queues.

## Requirements

Before attempting to run the worker project, there a few requirements.

### 1 - Ensure the local services are running

See: [Running the local services](./docker-compose.md)

### 2 - Set your local appSettings

The appSettings.Local.json file should be configured with the general project 
appSettings as needed for local development and committed to the git repo.

Any per-developer appSettings should be configured by the individual developer 
using user secrets.

## Starting the worker

Open the `server` solution in visual Studio or Jetbrains Rider and run 
the "Worker.Host" runtime configuration.

