# Project structure

## Server

### Api.Host

This is a RESTful API designed for the front-end NextJS application to use. Its primary purpose is to manage 
authentication and to facilitate data access to Cosmos DB.

### Worker.Host

This is a .Net worker service that will act as the consumer & worker handling incoming commands or events from the 
AMQP service.

It will also have handle the Cosmos DB change feed and send commands or publish events into the AMQP service as 
required.

### Infrastructure

This folder contains Class Library projects relating to the setup, configuration and usage of
3rd party services. See [Infrastructure](./infrastructure/index.md)

### Application

#### Application.Abstractions

Contains interfaces and abstract classes that can be used for dependency injection or implemented/inherited in other 
projects and allows other projects to require only the abstractions and not care about the implementations.

#### Application.CQRS

All commands, queries and notifications relating to the application along with their handlers where appropriate. 
Notification Handlers should be placed in the assembly that needs to handle the notification rather than in here.

#### Application.DtoModels

Simple DTOs for the application layer.

#### Application.Mappers

Mapper definitions for Entity > Application DTO.

#### Application.Messagemodels

This is a .Net Standard 2.0 Class Library project. .Net Standard was specifically chosen for its compatibility with 
the Net Framework 4.6.1 and up since we don't know what version of .Net 3rd parties might be using.

The idea of this project is that it may be deployed as a stand-alone nuget package so that 3rd parties that we are 
integrating with via the AMQP services can simple install the package and use exactly the same models as us.

Only POCO models should be located in this class library project.

### Domain

#### Domain.Abstractions

Contains interfaces and abstract classes that can be used for dependency injection or implemented/inherited in other 
projects and allows other projects to require only the abstractions and not care about the implementations.

#### Domain.Entities

Home of the entities.

## Client

A NextJS application utilizing the App Router. See https://nextjs.org/docs regarding using the App Router.
