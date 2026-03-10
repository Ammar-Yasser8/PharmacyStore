using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Specification
{
    public interface ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; }

        public Expression<Func<T, object>> OrderBy { get; }
        public Expression<Func<T, object>> OrderByDescending { get; }

        public int Take { get; }
        public int Skip { get; }
        public bool IsPagingEnabled { get; }

    }
}
