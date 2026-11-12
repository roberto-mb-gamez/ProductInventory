namespace ProductInventory.Application.Errors;
public sealed class NotFoundException(string message) : Exception(message);
