using Ardalis.GuardClauses;
using Domain.Entities;
using FluentValidation;

namespace Application.CQRS.Queries;

public sealed class GetExampleEntitiesPagedQuery : IQuery<OneOf<IPagedData<ExampleEntityDto>, ErrorDetails>>
{
    public GetExampleEntitiesPagedQuery(string? name, string? continuationToken = null)
    {

        Name = name;
        ContinuationToken = continuationToken;
    }

    public string? ContinuationToken { get; }
    public string? Name { get; }
}

public sealed class GetExampleEntitiesPagedQueryValidator : AbstractValidator<GetExampleEntitiesPagedQuery>
{
    public GetExampleEntitiesPagedQueryValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}

public sealed class GetExampleEntitiesPagedQueryHandler : IQueryHandler<GetExampleEntitiesPagedQuery,
        OneOf<IPagedData<ExampleEntityDto>, ErrorDetails>>
{
    private readonly ILogger<GetExampleEntitiesPagedQueryHandler> _logger;
    private readonly IRepository<ExampleEntity> _repository;

    public GetExampleEntitiesPagedQueryHandler(
        ILogger<GetExampleEntitiesPagedQueryHandler> logger,
        IRepository<ExampleEntity> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async ValueTask<OneOf<IPagedData<ExampleEntityDto>, ErrorDetails>> Handle(GetExampleEntitiesPagedQuery query, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { query });

        try
        {

            var result = string.IsNullOrEmpty(query.ContinuationToken)
                                                ? await _repository.QueryAsync(x => x.Name == query.Name, nameof(ExampleEntity), cancellationToken)
                                                : await _repository.ContinueQueryAsync(x => x.Name == query.Name, nameof(ExampleEntity), query.ContinuationToken, cancellationToken);

            return result.ToPagedDataDto();
        }
        catch(CosmosFriendlyException ex)
        {
            return new ErrorDetails(ex.Message);
        }

    }
}
