using ProductInventory.Domain;
using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Domain.Tests.Suppliers;
public sealed class SupplierTests
{
    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("Name <mail@example.test>")]
    [InlineData("a@@example.test")]
    public void SupplierRejectsInvalidProvidedEmail(string email)
    {
        Assert.Throws<ValidationException>(() => new Supplier(Guid.NewGuid(), "Supplier", email));
    }
    [Theory]
    [InlineData(null)]
    [InlineData("user@example.test")]
    public void SupplierAcceptsOptionalOrValidEmail(string? email)
    {
        Assert.Equal(email, new Supplier(Guid.NewGuid(), "Supplier", email).ContactEmail);
    }
    [Fact]
    public void SupplierRejectsMissingOrExcessiveName()
    {
        Assert.Throws<ValidationException>(() => new Supplier(Guid.NewGuid(), " ", null));
        Assert.Throws<ValidationException>(() => new Supplier(Guid.NewGuid(), new string('a', 151), null));
    }
}
