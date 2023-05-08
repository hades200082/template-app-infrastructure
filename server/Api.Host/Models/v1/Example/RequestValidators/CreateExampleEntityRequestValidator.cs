using Api.Host.Models.v1.Example.Requests;
using FluentValidation;

namespace Api.Host.Models.v1.Example.RequestValidators;

public sealed class CreateExampleEntityRequestValidator : AbstractValidator<CreateExampleEntityRequest>
{
    public CreateExampleEntityRequestValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}
