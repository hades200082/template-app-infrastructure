namespace Api.Host.Models.v1.UserProfile.Responses;

internal sealed record UserProfileResponseModel(
    string Id,
    string FirstName,
    string LastName,
    string Email
);
