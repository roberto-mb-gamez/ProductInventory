namespace ProductInventory.Domain;
public static class DomainRules
{
    public static string RequiredText(string value, int maximumLength, string field)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > maximumLength)
            throw new ValidationException($"El {field} es obligatorio y no debe exceder {maximumLength} caracteres.");

        return value;
    }
}
