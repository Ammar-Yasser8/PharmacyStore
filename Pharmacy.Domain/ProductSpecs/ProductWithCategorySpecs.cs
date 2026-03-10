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
        public ProductWithCategorySpecs() : base()
        {
            Includes.Add(p => p.Category);
        }
        public ProductWithCategorySpecs(int id) :base(p => p.Id == id)
        {
            Includes.Add(p => p.Category);
        }

    }
}
