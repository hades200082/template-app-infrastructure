namespace Application.CQRS.Queries.UserProfile;

public sealed record GetUserProfileByIdQuery(string Id) : IQuery<OneOf<UserProfileDto, NotFound, ErrorDetails>>;

public sealed class
    GetUserProfileByIdQueryHandler : IQueryHandler<GetUserProfileByIdQuery, OneOf<UserProfileDto, NotFound, ErrorDetails>>
{
    private readonly ILogger<GetUserProfileByIdQueryHandler> _logger;
    private readonly IRepository<Domain.Entities.UserProfile> _repository;

    public GetUserProfileByIdQueryHandler(
        ILogger<GetUserProfileByIdQueryHandler> logger,
        IRepository<Domain.Entities.UserProfile> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async ValueTask<OneOf<UserProfileDto, NotFound, ErrorDetails>> Handle(GetUserProfileByIdQuery query, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { query });

        try
        {
            var result = await _repository.FindAsync(query.Id, nameof(Domain.Entities.UserProfile), cancellationToken)
                .ConfigureAwait(false);

            if (result is null) return new NotFound();

            return result.ToDto();
        }
        catch // We don't care about the exception here, it's already been logged. We just want to return an Error.
        {
            return new ErrorDetails("Something went wrong when querying the user profile.");
        }

    }
}
