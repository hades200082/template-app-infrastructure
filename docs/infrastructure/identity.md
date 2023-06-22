# Infrastructure.Identity

This module adds Open ID Connect authentication capabilities to your application.

This should work with any Identity Service that supports OpenID Connect (OIDC). (i.e. Auth0, AzureAD, etc.)

## Usage & configuraton

Configure the module in the "Authentication" section of your appSettings:

See https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization/

```json
{
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "Authority": "https://leec-distinction-dev.eu.auth0.com",
        "ValidAudiences": ["https://leec-distinction.dev"],
        "ValidIssuer": "leec-distinction-dev.eu.auth0.com"
      }
    }
  }
}
```

In `Program.cs` add the Identity services to DI:

```csharp
builder.Services.AddIdentity(builder.Configuration);
```

Also add the identity middleware:

```csharp
app.UseIdentity();
```

## Extending

Additional features such as authorisation, roles, etc. can be setup in the Infrastructure.Identity class library as needed. 