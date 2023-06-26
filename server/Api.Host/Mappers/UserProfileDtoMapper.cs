using Api.Host.Models.v1.UserProfile.Responses;
using Application.DtoModels;
using Riok.Mapperly.Abstractions;

namespace Api.Host.Mappers;

[Mapper]
internal static partial class UserProfileDtoMapper
{
    public static partial CurrentUserProfileResponseModel ToCurrentUserProfileResponseModel(this UserProfileDto dto);
    public static partial UserProfileResponseModel ToResponseModel(this UserProfileDto dto);
}
