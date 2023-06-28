using Ardalis.GuardClauses;
using Domain.Entities;
using FluentValidation;

namespace Application.CQRS.Queries;

public sealed class GetExampleEntityQuery : IQuery<OneOf<ExampleEntityDto, NotFound>>
{
    public GetExampleEntityQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public sealed class GetExampleEntityQueryValidator : AbstractValidator<GetExampleEntityQuery>
{
    public GetExampleEntityQueryValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}

public sealed class
    GetExampleEntityQueryHandler : IQueryHandler<GetExampleEntityQuery,
        OneOf<ExampleEntityDto, NotFound>>
{
    private readonly ILogger<GetExampleEntityQueryHandler> _logger;
    private readonly IRepository<ExampleEntity> _repository;

    public GetExampleEntityQueryHandler(
        ILogger<GetExampleEntityQueryHandler> logger,
        IRepository<ExampleEntity> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async ValueTask<OneOf<ExampleEntityDto, NotFound>> Handle(GetExampleEntityQuery query, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { query });

        Guard.Against.NullOrWhiteSpace(query.Id);

        var entity = await _repository.FindAsync(query.Id, nameof(ExampleEntity), cancellationToken);
        if (entity is null) return new NotFound();

        return entity.ToDto();
    }
}
