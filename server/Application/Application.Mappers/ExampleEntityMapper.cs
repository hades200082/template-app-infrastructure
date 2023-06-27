using Application.MessageModels.Events;
using Shared.Core;

namespace Application.Mappers;

[Mapper]
public static partial class ExampleEntityMapper
{
    public static partial ExampleEntityCreated EntityToMessage(this ExampleEntity entity);
    public static partial ExampleEntityDto ToDto(this ExampleEntity entity);

    public static partial PagedData<ExampleEntityDto> ToPagedDataDto(this IPagedData<ExampleEntity> entity);
}
