using Application.MessageModels.Events;

namespace Application.Mappers;

[Mapper]
public sealed partial class ExampleEntityMapper
{
    public partial ExampleEntityCreated EntityToMessage(ExampleEntity entity);
    public partial ExampleEntityDto EntityToDto(ExampleEntity entity);
}
