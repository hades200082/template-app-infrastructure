namespace Application.DtoModels;

public sealed record UserProfileDto(
    string Id,
    string FirstName,
    string LastName,
    string Email
);
