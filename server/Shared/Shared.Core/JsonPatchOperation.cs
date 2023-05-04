using System.ComponentModel.DataAnnotations;

namespace Shared.Core;

public class JsonPatchOperation
{
    [Required]
    public string Op { get; set; }

    [Required]
    public string Path { get; set; }

    public object? Value { get; set; }

    // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    // {
    //     if (string.IsNullOrWhiteSpace(Op))
    //         yield return new ValidationResult($"Operation is required.");
    //
    //     var validOps = new[] { "add", "remove", "replace", "set" };
    //     if (string.IsNullOrWhiteSpace(Op))
    //         yield return new ValidationResult("Operation is required");
    //
    //     if (!validOps.Contains(Op, StringComparer.OrdinalIgnoreCase))
    //         yield return new ValidationResult("Invalid operation");
    //
    //     if (!Path.StartsWith("/", StringComparison.Ordinal))
    //         yield return new ValidationResult("Path must start with a forward slash");
    //
    //     if(!Op.Equals("remove", StringComparison.OrdinalIgnoreCase) && Value is null)
    //         yield return new ValidationResult("A value must be provided for add or replace operations");
    //
    // }
}
