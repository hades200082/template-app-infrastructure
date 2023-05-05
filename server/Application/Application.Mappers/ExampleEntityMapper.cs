using Application.DtoModels;
using Application.MessageModels.Events;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

[Mapper]
public sealed partial class ExampleEntityMapper
{
    public partial ExampleEntityCreated EntityToMessage(ExampleEntity entity);
    public partial ExampleEntityDto EntityToDto(ExampleEntity entity);
}
