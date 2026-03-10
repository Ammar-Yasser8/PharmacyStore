using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Specification
{
    public class BaseSpecification<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; set; } = null;

        public List<Expression<Func<T, object>>> Includes {  get; set; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        public BaseSpecification()
        {
            
        }
        public BaseSpecification(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            IsPagingEnabled = true;
            Skip = skip;
            Take = take;
        }
    }
}
