namespace Api.Host.Models.v1.Example.Requests
{
    public record GetExampleEntitiesPagedRequest(string? Name, string? ContinuationToken);
}
