namespace Application.CQRS.Queries.UserProfile;

public sealed record GetUserProfileByExternalIdQuery(string ExternalId) : IQuery<OneOf<UserProfileDto, NotFound, ErrorDetails>>;

public sealed class
    GetUserProfileByExternalIdQueryHandler : IQueryHandler<GetUserProfileByExternalIdQuery, OneOf<UserProfileDto, NotFound, ErrorDetails>>
{
    private readonly ILogger<GetUserProfileByExternalIdQueryHandler> _logger;
    private readonly IRepository<Domain.Entities.UserProfile> _repository;

    public GetUserProfileByExternalIdQueryHandler(
        ILogger<GetUserProfileByExternalIdQueryHandler> logger,
        IRepository<Domain.Entities.UserProfile> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async ValueTask<OneOf<UserProfileDto, NotFound, ErrorDetails>> Handle(GetUserProfileByExternalIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { query });

        try
        {
            var result = await _repository.QueryAsync(
                    x => x.ExternalId == query.ExternalId,
                    nameof(Domain.Entities.UserProfile),
                    1,
                    cancellationToken)
                .ConfigureAwait(false);

            if (result.TotalCount == 0 || !result.Data.Any()) return new NotFound();

            return result.Data.First().ToDto();
        }
        catch // We don't care about the exception type here, it's already been logged. We just want to return an Error.
        {
            return new ErrorDetails("Something went wrong when querying the user profile.");
        }
    }
}
