using Api.Host.Models.v1.Example.Requests;
using Application.CQRS.Queries;
using Riok.Mapperly.Abstractions;

namespace Api.Host.Mappers;

[Mapper]
public static partial class GetExampleEntitiesPagedRequestMapper
{
    public static partial GetExampleEntitiesPagedQuery ToQuery(this GetExampleEntitiesPagedRequest obj);
}
