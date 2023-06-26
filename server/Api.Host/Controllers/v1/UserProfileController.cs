using Api.Host.Mappers;
using Api.Host.Models.v1.UserProfile.Responses;
using Application.CQRS.Queries.UserProfile;
using Infrastructure.Identity;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Shared.Core;

namespace Api.Host.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json, "text/json")]
public sealed class UserProfileController : ControllerBase
{
    private readonly ILogger<UserProfileController> _logger;
    private readonly IMediator _mediator;

    public UserProfileController(
        ILogger<UserProfileController> logger,
        IMediator mediator
    )
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get the user profile data for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Found - Body will contain the <see cref="CurrentUserProfileResponseModel"/> object</response>
    /// <response code="400">Validation failed - Body will contain a <see cref="ValidationProblemDetails"/> object</response>
    /// <response code="404">User profile not found</response>
    /// <response code="500">Something went wrong when querying the database - Body will contain a <see cref="ProblemDetails"/> object</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserProfileResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentUserProfileAsync(CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(null);

        // While the [Authorize] attribute ensures that we have a valid user
        // We need to validate that the user has an external ID we can use to
        // query the database with
        var authId = User.ExternalId();
        if (string.IsNullOrWhiteSpace(authId))
        {
            _logger.LogExternalIdError(User);
            return ValidationProblem("No external user ID present. Unable to retrieve a profile.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var queryResult = await _mediator.Send(new GetUserProfileByExternalIdQuery(authId), cancellationToken)
            .ConfigureAwait(false);

        return queryResult.Match<IActionResult>(
            x => Ok(x.ToCurrentUserProfileResponseModel()),
            _ => NotFound(),
            error => Problem(error.Details, statusCode: StatusCodes.Status500InternalServerError)
        );
    }

    /// <summary>
    /// Get the user profile data for the user with the given ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Found - Body will contain the <see cref="UserProfileResponseModel"/> object</response>
    /// <response code="400">Validation failed - Body will contain a <see cref="ValidationProblemDetails"/> object</response>
    /// <response code="404">User profile not found</response>
    /// <response code="500">Something went wrong when querying the database - Body will contain a <see cref="ProblemDetails"/> object</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CurrentUserProfileResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType( StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserProfileAsync(string id, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { id });

        /* ***********************************************************************************************************
         * TODO: Review this action and add checks to ensure current user is allowed to access requested user profile
         *
         *    We probably want to get the current logged in user here and check that
         *    they are either requesting their own profile, or have the relevant permissions
         *    to query other users' profiles
         * *********************************************************************************************************** */

        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var _))
        {
            _logger.LogExternalIdError(User);
            return ValidationProblem("The User ID requested was invalid. Unable to retrieve a profile.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var queryResult = await _mediator.Send(new GetUserProfileByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);

        return queryResult.Match<IActionResult>(
            x => Ok(x.ToResponseModel()),
            _ => NotFound(),
            error => Problem(error.Details, statusCode: StatusCodes.Status500InternalServerError)
        );
    }
}
