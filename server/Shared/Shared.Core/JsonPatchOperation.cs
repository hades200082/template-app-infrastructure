namespace Shared.Core;

public sealed class JsonPatchOperation
{
    public JsonPatchOperation(string op, string path, object? value)
    {
        Op = op;
        Path = path;
        Value = value;
    }

    public string Op { get; }

    public string Path { get; }

    public object? Value { get; }
}
