namespace StellarPageable.Models
{

    public class PaginatedRequest
    {
        public string? Filter { get; set; }
        public string? OrderBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
