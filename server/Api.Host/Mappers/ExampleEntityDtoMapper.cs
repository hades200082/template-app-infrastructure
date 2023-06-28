using Api.Host.Models.v1.Example;
using Application.DtoModels;
using Riok.Mapperly.Abstractions;
using Shared.Core;

namespace Api.Host.Mappers;

[Mapper]
internal static partial class ExampleEntityDtoMapper
{
    public static partial ExampleEntityResponseModel ToResponse(this ExampleEntityDto entity);
    public static partial PagedData<ExampleEntityResponseModel> ToPagedResponse(this IPagedData<ExampleEntityDto> entity);
}
