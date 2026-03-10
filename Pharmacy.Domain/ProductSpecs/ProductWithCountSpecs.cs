using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Specification;

namespace Pharmacy.Domain.ProductSpecs
{
    public class ProductWithCountSpecs : BaseSpecification<Product>
    {
        public ProductWithCountSpecs(ProductSpecParams specParams)
            : base(p =>
                (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value) &&
                (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search))
            )
        {
        }
    }
}
