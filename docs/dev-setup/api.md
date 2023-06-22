# Running the API

## Requirements

Before attempting to run the API there a few requirements.

### 1 - Ensure the local services are running

See: [Running the local services](./docker-compose.md)

### 2 - Set your local appSettings

The appSettings.Local.json file should be configured with the general project appSettings as 
needed for local development and committed to the git repo.

Any per-developer appSettings should be configured by the individual developer using user secrets.

## Starting the API

Open the `server` solution in visual Studio or Jetbrains Rider and run 
the "Api.Host: https" runtime configuration.

> ***Note:** Swagger is configured in the `API.Host` project to provide ReDoc documentation.*
>
> - *ReDoc: https://localhost:7155/api-docs/*
> 
> This URL will open by default when you run the API.Host project.

## Testing the API

I recommend using a [Postman](https://www.postman.com/) project to document and test your API.

You can import the OpenAPI JSON definition from the ReDoc interface into Postman if required.
