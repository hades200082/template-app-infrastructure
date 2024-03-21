using Domain.Entities;

namespace Application.CQRS.Commands;

/*
 * By convention we will create everything relating to this command in this file.
 * This includes the command, its handler and its validator
 */

public sealed class CreateExampleEntityCommand : ICommand<OneOf<ExampleEntityDto, Error<string>>>
{
    public CreateExampleEntityCommand(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public sealed class CreateExampleEntityCommandHandler : ICommandHandler<CreateExampleEntityCommand, OneOf<ExampleEntityDto, Error<string>>>
{
    private readonly ILogger<CreateExampleEntityCommandHandler> _logger;
    private readonly IRepository<ExampleEntity> _repository;

    public CreateExampleEntityCommandHandler(
        ILogger<CreateExampleEntityCommandHandler> logger,
        IRepository<ExampleEntity> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async ValueTask<OneOf<ExampleEntityDto, Error<string>>> Handle(CreateExampleEntityCommand command, CancellationToken cancellationToken)
    {
        _logger.LogMethodCall(new { command });

        // Should be validated in controller before getting here so throw is fine
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name, nameof(command));

        var entity = await _repository.CreateAsync(new ExampleEntity(command.Name), cancellationToken);

        // If the returned value is null then we had an error in Cosmos
        // This shouldn't happen and will already have been logged in the repository,
        // but it's safer to check than to return a null from a create.
        if (entity is null) return new Error<string>("Create command failed. Please retry.");

        return new ExampleEntityMapper().EntityToDto(entity);
    }
}
