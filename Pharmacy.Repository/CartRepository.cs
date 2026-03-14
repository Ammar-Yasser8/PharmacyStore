using Microsoft.EntityFrameworkCore;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Repository.Data;
using System.Threading.Tasks;

namespace Pharmacy.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly PharmacyDBContext _context;

        public CartRepository(PharmacyDBContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartAsync(string cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
