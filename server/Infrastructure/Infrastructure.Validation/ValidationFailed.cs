using FluentValidation.Results;

namespace Infrastructure.Validation;

public sealed record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(ValidationFailure error) : this(new[] { error }){}
}
