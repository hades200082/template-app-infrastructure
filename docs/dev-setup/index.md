# Developer Setup

1. [Running the local services](./docker-compose.md)
2. [Running the API](./api.md)
3. [Running the Worker](./worker.md)
4. [Running the front-end web-app](./client.md)


### Starting the client web-application

1. Ensure that the API and Worker are both running (see above)
2. Open the [client](../client) folder in your terminal/CLI
3. Ensure that all dependencies are installed using `npm install`
4. Start the NextJS application using `npm run dev`

## Infrastructure Components

A number of the features of this template are controlled via class libraries `Infrastructure.*`.

See [Infrastructure Components](./infrastructure/index.md) for more info

## Troubleshooting

### Cosmos DB Emulator is not working
The docker image for the cosmos DB emulator has a built-in evaluation period after which time the emulator container will fail to start. If you check the logs you will see a message stating "Error: The evaluation period has expired.". This is discussed in a github issue at https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/60

To fix this issue and get up and running again run the following commands:

```bash
docker compose down
docker compose pull
docker compose up -d
```

This removes the docker containers, pulls the latest images needed for the respective services, then recreates and starts the containers.

> ***Note:** this has the potential to introduce bugs if the services are updated with changes that break backwards compatibility.*
