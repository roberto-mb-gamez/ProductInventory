using ProductInventory.Domain;
namespace ProductInventory.Application.Pagination;
public sealed record PageRequest(int Page = 1, int PageSize = 20)
{
    public long Offset => ((long)Page - 1) * PageSize;
    public void Validate()
    {
        if (Page < 1 || PageSize is < 1 or > 100)
            throw new ValidationException("La página debe ser al menos 1 y el tamaño debe estar entre 1 y 100.");
    }
}
