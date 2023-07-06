using Application.CQRS.Queries;
using Application.DtoModels;
using Domain.Entities;
using Infrastructure.Cosmos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OneOf.Types;

namespace Application.CQRS.Tests;

public class GetExampleEntityQueryTests
{
    [Fact]
    public async void HandlerReturnsCorrectlyMappedDto()
    {
        var logger = Substitute.For<ILogger<GetExampleEntityQueryHandler>>();

        var repository = Substitute.For<IRepository<ExampleEntity>>();
        repository.FindAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ExampleEntity("Test Entity"){  Id = "test-id" });

        var query = new GetExampleEntityQuery("test-id");
        var sut = new GetExampleEntityQueryHandler(logger, repository);
        var result = await sut.Handle(query, default);

        // Verify that the handler called the repository FindAsync method.
        await repository.Received().FindAsync("test-id", nameof(ExampleEntity), default);

        var item = result.Match(x => x, _ =>
        {
            Assert.Fail("NotFound result returned");
            return null;
        });

        Assert.NotNull(item);
        Assert.IsType<ExampleEntityDto>(item);
        Assert.Equal("test-id", item.Id);
        Assert.Equal("Test Entity", item.Name);
    }

    [Fact]
    public async void HandlerReturnsNotFoundForNullResult()
    {
        var logger = Substitute.For<ILogger<GetExampleEntityQueryHandler>>();

        var repository = Substitute.For<IRepository<ExampleEntity>>();
        repository.FindAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ReturnsNullForAnyArgs();

        var query = new GetExampleEntityQuery("test-id");
        var sut = new GetExampleEntityQueryHandler(logger, repository);
        var result = await sut.Handle(query, default);

        // Verify that the handler called the repository FindAsync method.
        await repository.Received().FindAsync("test-id", nameof(ExampleEntity), default);

        result.Match<ExampleEntityDto?>(
            x =>
            {
                Assert.Fail("Item returned from notfound test");
                return null;
            },
            y =>
            {
                Assert.IsType<NotFound>(y);
                return null;
            }
        );
    }
}
