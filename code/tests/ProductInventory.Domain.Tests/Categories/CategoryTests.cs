using ProductInventory.Domain;
using ProductInventory.Domain.Categories;
namespace ProductInventory.Domain.Tests.Categories;
public sealed class CategoryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t ")]
    public void CategoryRejectsMissingName(string? name)
    {
        Assert.Throws<ValidationException>(() => new Category(Guid.NewGuid(), name!, null));
    }
    [Fact]
    public void CategoryRejectsNameOverOneHundredCharacters()
    {
        Assert.Throws<ValidationException>(() => new Category(Guid.NewGuid(), new string('a', 101), null));
    }
    [Fact]
    public void CategoryPreservesValidData()
    {
        var id = Guid.NewGuid();
        var category = new Category(id, "Tools", "Hand tools");
        Assert.Equal(id, category.Id);
        Assert.Equal("Tools", category.Name);
        Assert.Equal("Hand tools", category.Description);
    }
}
