using Moq;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Errors;
using ProductInventory.Application.Pagination;
using ProductInventory.Domain;
using ProductInventory.Domain.Categories;
namespace ProductInventory.Application.Tests.Categories;
public sealed class CategoryHandlerTests
{
    [Fact]
    public async Task CreateDelegatesValidatedDataToWriter()
    {
        var writer = new Mock<ICategoryWriter>();
        writer.Setup(port => port.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category category, CancellationToken _) => new CategoryDto(category.Id, category.Name, category.Description));
        var result = await new CreateCategoryHandler(writer.Object).HandleAsync(new(new("Tools", null)));
        Assert.Equal("Tools", result.Name);
        writer.Verify(port => port.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task InvalidCreateNeverCallsPersistence()
    {
        var writer = new Mock<ICategoryWriter>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new CreateCategoryHandler(writer.Object).HandleAsync(new(new(" ", null))));
        writer.VerifyNoOtherCalls();
    }
    [Fact]
    public async Task MissingGetAndUpdateReturnNotFound()
    {
        var reader = new Mock<ICategoryReader>();
        var writer = new Mock<ICategoryWriter>();
        await Assert.ThrowsAsync<NotFoundException>(() => new GetCategoryHandler(reader.Object).HandleAsync(new(Guid.NewGuid())));
        await Assert.ThrowsAsync<NotFoundException>(() => new UpdateCategoryHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), new("Tools", null))));
    }
    [Fact]
    public async Task MissingDeleteReturnsNotFound()
    {
        var writer = new Mock<ICategoryWriter>();
        await Assert.ThrowsAsync<NotFoundException>(() => new DeleteCategoryHandler(writer.Object).HandleAsync(new(Guid.NewGuid())));
    }
    [Fact]
    public async Task InvalidPageNeverCallsReader()
    {
        var reader = new Mock<ICategoryReader>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new ListCategoriesHandler(reader.Object).HandleAsync(new(new PageRequest(0, 20))));
        reader.VerifyNoOtherCalls();
    }
}
