namespace ProductInventory.Domain;
public sealed class ValidationException(string message) : Exception(message);
