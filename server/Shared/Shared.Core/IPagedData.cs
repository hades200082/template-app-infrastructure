namespace Shared.Core;

public interface IPagedData<out TData>
{
    IEnumerable<TData> Data { get; }
    string? ContinuationToken { get; }
    bool HasMore { get; }
    int TotalCount { get; }
}
