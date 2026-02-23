namespace MiniWebApp.Core.Models;

public record PagedRequest
{
    private const int MaxPageSize = 100;
    private readonly int _pageNumber = 1;
    private readonly int _pageSize = 10;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
    }
    public void Deconstruct(out int pageNumber, out int pageSize)
    {
        pageNumber = PageNumber;
        pageSize = PageSize;
    }
}