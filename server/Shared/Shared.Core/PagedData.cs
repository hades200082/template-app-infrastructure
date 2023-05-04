namespace Shared.Core;

public class PagedData<TData> : IPagedData<TData>
{
    public PagedData(IEnumerable<TData> data, string? continuationToken, bool hasMore, int totalCount)
    {
        Data = data;
        ContinuationToken = continuationToken;
        HasMore = hasMore;
        TotalCount = totalCount;
    }

    public IEnumerable<TData> Data { get; }
    public string? ContinuationToken { get; }
    public bool HasMore { get; }
    public int TotalCount { get; }
}
