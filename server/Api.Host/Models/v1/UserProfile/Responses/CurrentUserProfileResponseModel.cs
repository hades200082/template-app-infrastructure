namespace Api.Host.Models.v1.UserProfile.Responses;

internal sealed record CurrentUserProfileResponseModel(
    string Id,
    string FirstName,
    string LastName,
    string Email
);
