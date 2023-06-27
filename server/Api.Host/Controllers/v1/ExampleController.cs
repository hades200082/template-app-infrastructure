using System.ComponentModel.DataAnnotations;
using Api.Host.Mappers;
using Api.Host.Models.v1.Example;
using Api.Host.Models.v1.Example.Requests;
using Application.CQRS.Commands;
using Application.CQRS.Queries;
using FluentValidation;
using Mediator;
using Microsoft.Azure.Cosmos.Linq;
using Shared.Core;

namespace Api.Host.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json, "text/json")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateExampleEntityRequest> _createValidator;

    public ExampleController(
        ILogger<ExampleController> logger,
        IMediator mediator,
        IValidator<CreateExampleEntityRequest> createValidator)
    {
        _logger = logger;
        _mediator = mediator;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Get by ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Found</response>
    /// <response code="400">Validation failed</response>
    /// <response code="404">Object not found</response>
    [HttpGet("{id}", Name = "Find")]
    [ProducesResponseType(typeof(ExampleEntityResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([Required]string id, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { id });
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var queryResult = await _mediator.Send(new GetExampleEntityQuery(id), cancellationToken).ConfigureAwait(false);
        
        return queryResult.Match<IActionResult>(
            x => Ok(x.ToResponse()),
            _ => NotFound()
        );
    }

    /// <summary>
    /// Get by Name
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="continuationToken"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Found</response>
    /// <response code="400">Validation failed</response>
    /// <response code="500">Error</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedData<ExampleEntityResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPageAsync([FromQuery] GetExampleEntitiesPagedRequest model, CancellationToken cancellationToken)
    {
        _logger.LogControllerRequestTrace(ControllerContext.RouteData);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (model == null)
            return Problem("All values can't be null", statusCode: StatusCodes.Status400BadRequest);

        var queryResult = await _mediator.Send(new GetExampleEntitiesPagedQuery(model.Name, model.ContinuationToken), cancellationToken).ConfigureAwait(false);

        return queryResult.Match<IActionResult>(
            x => Ok(x.ToPagedResponse()),
            e => Problem(e.Details, statusCode: StatusCodes.Status500InternalServerError)
        );
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Created successfully</response>
    /// <response code="400">Validation failed</response>
    /// <response code="500">Something went wrong</response>
    [HttpPost]
    [Consumes(typeof(CreateExampleEntityRequest), MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ExampleEntityResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        CreateExampleEntityRequest request, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { request });

        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var createResult = await _mediator
            .Send(new CreateExampleEntityCommand(request.Name), cancellationToken)
            .ConfigureAwait(false);

        return createResult.Match<IActionResult>(
            x => CreatedAtRoute( "Find", new { x.Id }, x.ToResponse()),
            error => Problem(error.Value, statusCode: StatusCodes.Status500InternalServerError)
        );
    }
}
