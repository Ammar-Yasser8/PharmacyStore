using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain.ProductSpecs
{
    public class ProductWithCategorySpecs : BaseSpecification<Product>
    {
        public ProductWithCategorySpecs(ProductSpecParams specParams)
            : base(p =>
                (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value) &&
                (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search))
            )
        {
            Includes.Add(p => p.Category);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort.ToLower())
                {
                    case "nameasc":
                        AddOrderBy(p => p.Name);
                        break;
                    case "namedesc":
                        AddOrderByDescending(p => p.Name);
                        break;
                    case "priceasc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "pricedesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }

            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }

        public ProductWithCategorySpecs(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Category);
        }

    }
}
