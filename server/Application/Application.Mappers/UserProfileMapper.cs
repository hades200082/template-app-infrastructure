namespace Application.Mappers;

[Mapper]
public static partial class UserProfileMapper
{
    public static partial UserProfileDto ToDto(this UserProfile entity);
}
