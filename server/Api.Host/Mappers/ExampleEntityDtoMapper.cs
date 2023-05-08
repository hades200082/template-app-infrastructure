using Api.Host.Models.v1.Example;
using Application.DtoModels;
using Riok.Mapperly.Abstractions;

namespace Api.Host.Mappers;

[Mapper]
internal sealed partial class ExampleEntityDtoMapper
{
    public partial ExampleEntityResponseModel ToResponse(ExampleEntityDto entity);
}
