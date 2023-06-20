# Infrastructure.Identity

This module adds Open ID Connect authentication capabilities to your application.

This should work with any Identity Service that supports OpenID Connect (OIDC). (i.e. Auth0, AzureAD, etc.)

## Usage & configuraton

Configure the module in your appSettings using the JSON below (remove the comments):

```json
{
  "Identity": {
    "ClientSecret": "", // The Client Secret from your identity service 
    "ClientId": "", // The client ID from your identity service
    "Authority": "", // The URL of your tenant's Authority
    
    // Below are optional properties that you may or may not want/need 
    // depending on your applications requirements and your 
    // Identity Service of choice
    
    // The list of scopes being requested from the Identity Service
    // "openid" is required, others are optional.
    "Scope":[ 
      "openid",
      "profile",
      "email"
    ],
    "TokenValidationParameters": {
      "NameClaimType": "name", // The Claim Type of the "username" for the user, set to "name" for most providers
      "ValidateAudience": true, // If true (default) then ValidAudiance must be set, and configured in the Identity Service
      "ValidateIssuer": true, // If true (default) then at least one ValidIssuer is required
      "ValidAudience": "",
      "ValidIssuers": []
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