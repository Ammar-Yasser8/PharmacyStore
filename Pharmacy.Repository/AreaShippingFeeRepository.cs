using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Repository.Data;

namespace Pharmacy.Repository
{
    public class AreaShippingFeeRepository : IAreaShippingFeeRepository
    {
        private readonly PharmacyDBContext _context;

        public AreaShippingFeeRepository(PharmacyDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AreaShippingFee areaShippingFee)
        {
            await _context.AreaShippingFees.AddAsync(areaShippingFee);
        }

        public void Delete(AreaShippingFee areaShippingFee)
        {
            _context.AreaShippingFees.Remove(areaShippingFee);
        }

        public async Task<IReadOnlyList<AreaShippingFee>> GetAllAsync()
        {
            return await _context.AreaShippingFees.ToListAsync();
        }

        public async Task<AreaShippingFee?> GetByAreaAsync(string area)
        {
            return await _context.AreaShippingFees.FirstOrDefaultAsync(x => x.Area == area);
        }

        public async Task<AreaShippingFee?> GetByIdAsync(int id)
        {
            return await _context.AreaShippingFees.FindAsync(id);
        }

        public void Update(AreaShippingFee areaShippingFee)
        {
            _context.AreaShippingFees.Update(areaShippingFee);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
