# Infrastructure.Identity

This lays a foundation for implementing custom integrations with any Identity Provider in the future.

## Usage & configuration

Configure the selected identity provider's Options in your appSettings. 

For Auth0, for example, there are two sets of options. One for authentication and one
for the Management API. you only need include the section relevant to what you are using:

```json
{
  "Auth0Options": {
    "Domain": "leec-distinction-dev.eu.auth0.com",
    "ValidAudiences": ["https://leec-distinction.dev"],
    "NameClaimType": "name"
  },
  "Auth0ManagementApiOptions": {
    "Domain": "leec-distinction-dev.eu.auth0.com",
    "ClientId": "dkfjghadfkljghadflkjhgdfgkjh",
    "ClientSecret": "sdflkjghalkjdfshgakjfhgadfgjh"
  }
}
```

In `Program.cs` add the chosen identity services to DI. Note that each is independent of the others.:

```csharp
builder.Services.AddAuth0(builder.Configuration); // Adds Auth0 authentication with JWT
builder.Services.AddAuth0AuthenticationApiClient(builder.Configuration); // Adds the Auth0 AuthenticationApiClient
builder.Services.AddAuth0ManagementApiClient(builder.Configuration); // Adds the Auth0 ManagementApiClient (including taking care of getting/refreshing a token automatically)
```

Also add the identity middleware to the API host if required:

```csharp
app.UseCustomAuthentication();
```

## Extending

Additional features such as authorisation, roles, etc. can be setup in the Infrastructure.Identity class library as needed. 