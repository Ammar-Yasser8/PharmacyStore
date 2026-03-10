using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Domain.Specification;
using Pharmacy.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly PharmacyDBContext _context;
        public GenericRepository(PharmacyDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable <T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);   
        }

        public async Task<T?> GetWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        // method for reducing code duplication in the above methods
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }
    }
}
