# Requirements

## Required packages & software

- .Net 7 SDK
- NodeJS 18+ & NPM 9+
- Docker (with docker compose)

## Required knowledge

To make the most from this repository it's advisable to have a good understanding of the following technologies and patters:

### General

- Docker Compose
- Auth0

### Server

- Azure Cosmos DB - NoSQL/Document DB architecture
  - Change feed
- Azure storage
- Generic Repository pattern for data access
- Mediator pattern and general CQRS principles
- Performant logging with LoggerMessage.Define
- .Net Core
  - WebAPI and RESTful principles
  - API versioning
  - Authentication & Authorization
  - IHostedService for workers
- Discriminated unions
- AMQP / Message queues / Event driven architecture
  - MassTransit
  - RabbitMQ for local development
  - Any transport supported by MassTransit for hosted environments

### Client

- TypeScript
- Runtime type checking with Zod
- NextJS 13
- React 18 and React Server Components
- RESTful principles for working with APIs
