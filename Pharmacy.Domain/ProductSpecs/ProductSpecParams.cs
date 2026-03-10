namespace Pharmacy.Domain.ProductSpecs
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 5;

        public string? Sort { get; set; }
        public int? CategoryId { get; set; }

        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.Trim().ToLower();
        }

        public int PageIndex { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
