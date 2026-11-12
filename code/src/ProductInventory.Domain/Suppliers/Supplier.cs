using System.Net.Mail;
namespace ProductInventory.Domain.Suppliers;
public sealed class Supplier(Guid id, string name, string? contactEmail)
{
    public Guid Id { get; } = id;
    public string Name { get; } = DomainRules.RequiredText(name, 150, "nombre de proveedor");
    public string? ContactEmail { get; } = ValidateEmail(contactEmail);
    private static string? ValidateEmail(string? value)
    {
        if (value is not null && (!MailAddress.TryCreate(value, out var address) || address.Address != value))
            throw new ValidationException("El correo de contacto no es válido.");

        return value;
    }
}
