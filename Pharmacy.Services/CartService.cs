using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public CartService(ICartRepository cartRepository, IGenericRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart?> GetCartAsync(string cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            return cart;
        }

        public async Task<Cart?> AddItemAsync(string cartId, int productId, int quantity)
        {
            if (quantity <= 0) return null;

            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null)
            {
                cart = new Cart { Id = cartId };
                await _cartRepository.AddCartAsync(cart);
            }

            var product = await _productRepository.GetAsync(productId);
            if (product == null) return null; // product not found

            if (quantity > product.Stock) return null; // insufficient stock

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > product.Stock) return null; // stock check
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _cartRepository.SaveChangesAsync();

            // Refresh cart to get the included product data
            return await _cartRepository.GetCartAsync(cartId);
        }

        public async Task<Cart?> UpdateItemQuantityAsync(string cartId, int productId, int quantity)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem == null) return null;

            if (quantity <= 0)
            {
                cart.Items.Remove(existingItem);
            }
            else
            {
                var product = await _productRepository.GetAsync(productId);
                if (product == null) return null;
                if (quantity > product.Stock) return null;

                existingItem.Quantity = quantity;
            }

            await _cartRepository.SaveChangesAsync();

            return await _cartRepository.GetCartAsync(cartId);
        }

        public async Task<Cart?> RemoveItemAsync(string cartId, int productId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                cart.Items.Remove(existingItem);
                await _cartRepository.SaveChangesAsync();
            }

            return await _cartRepository.GetCartAsync(cartId);
        }

        public async Task<Cart?> ClearCartAsync(string cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            cart.Items.Clear();
            await _cartRepository.SaveChangesAsync();

            return await _cartRepository.GetCartAsync(cartId);
        }
    }
}
