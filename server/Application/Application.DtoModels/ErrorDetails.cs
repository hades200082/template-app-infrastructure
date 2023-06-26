using System.Runtime.InteropServices;

namespace Application.DtoModels;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct ErrorDetails : IEquatable<ErrorDetails>
{
    public ErrorDetails(string details)
    {
        Details = details;
    }

    public string Details { get; }

    public override bool Equals(object? obj)
    {
        return Equals((ErrorDetails)(obj ?? new ErrorDetails("")));
    }

    public override int GetHashCode()
    {
        return Details.GetHashCode(StringComparison.Ordinal);
    }

    public static bool operator ==(ErrorDetails left, ErrorDetails right)
    {
        return left.Details.Equals(right.Details, StringComparison.Ordinal);
    }

    public static bool operator !=(ErrorDetails left, ErrorDetails right)
    {
        return left.Details != right.Details;
    }

    public bool Equals(ErrorDetails other)
    {
        return this == other;
    }
}
