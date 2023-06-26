using Domain.Abstractions;

namespace Domain.Entities;

public sealed class UserProfile : Entity
{
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
