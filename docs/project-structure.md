# Project structure

## Server

### Api.Host

This is a RESTful API designed for the front-end NextJS application to use. Its primary purpose is to manage authentication and to facilitate data access to Cosmos DB.

### Worker.Host

This is a .Net worker service that will act as the consumer & worker handling incoming commands or events from the AMQP service.

It will also have handle the Cosmos DB change feed and send commands or publish events into the AMQP service as required.

### Infrastructure

This folder contains Class Library projects relating to the setup, configuration and usage of
3rd party services.

### Shared

#### Infrastructure.Cosmos

Registers and configures Azure Cosmos DB and defines:

- A generic repository for data access
- A custom interface `IChangeFeedProvider` and default implementation which registers a change feed handler
- Custom LoggerMessage definitions specific to Cosmos

#### Infrastructure.AQMP

Registers MassTransit for use with an AMQP transport.

Currently configured to automatically reghister RabbitMQ as the local development transport and Azure Service Bus for all other environments.

#### Infrastructure.Logging

Registers Serilog.

If a `SentryDsn` appSetting is provided also registers Sentry.io integration.

#### Infrastructure.Storage

Registers the `BlobServiceClient` from the Azure Storage SDK.

#### Infrastructure.Validation

Registers all `IValidator` instances from all loaded assemblies for use with FluentValidation.

### Application

#### Application.Abstractions

TBC

#### Application.CQRS

TBC

#### Application.DtoModels

TBC

#### Application.Mappers

TBC

#### Application.Messagemodels

This is a .Net Standard 2.0 Class Library project. .Net Standard was specifically chosen for its compatibility with the Net Framework 4.6.1 and up since we don't know what version of .Net 3rd parties might be using.

The idea of this project is that it may be deployed as a stand-alone nuget package so that 3rd parties that we are integrating with via the AMQP services can simple install the package and use exactly the same models as us.

Only POCO models should be located in this class library project.

### Domain

TBC

## Client

A NextJS application utilizing the App Router. See https://nextjs.org/docs regarding using the App Router.
