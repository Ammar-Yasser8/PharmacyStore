using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Services.Dtos.CartDtos;
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

        public async Task<CartToReturnDto?> GetCartAsync(string cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            return MapCartToReturnDto(cart);
        }

        public async Task<CartToReturnDto?> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            return MapCartToReturnDto(cart);
        }

        public async Task AssignCartToUserAsync(string cartId, string userId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null)
            {
                cart = new Cart { Id = cartId, AppUserId = userId };
                await _cartRepository.AddCartAsync(cart);
            }
            else
            {
                cart.AppUserId = userId;
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task<CartToReturnDto?> AddItemAsync(string cartId, int productId, int quantity)
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
            var updatedCart = await _cartRepository.GetCartAsync(cartId);
            if (updatedCart == null) return null;

            return MapCartToReturnDto(updatedCart);
        }

        public async Task<CartToReturnDto?> UpdateItemQuantityAsync(string cartId, int productId, int quantity)
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

            var updatedCart = await _cartRepository.GetCartAsync(cartId);
            if (updatedCart == null) return null;

            return MapCartToReturnDto(updatedCart);
        }

        public async Task<CartToReturnDto?> RemoveItemAsync(string cartId, int productId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                cart.Items.Remove(existingItem);
                await _cartRepository.SaveChangesAsync();
            }

            var updatedCart = await _cartRepository.GetCartAsync(cartId);
            if (updatedCart == null) return null;

            return MapCartToReturnDto(updatedCart);
        }

        public async Task<CartToReturnDto?> ClearCartAsync(string cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null) return null;

            cart.Items.Clear();
            await _cartRepository.SaveChangesAsync();

            var updatedCart = await _cartRepository.GetCartAsync(cartId);
            if (updatedCart == null) return null;

            return MapCartToReturnDto(updatedCart);
        }

        private CartToReturnDto MapCartToReturnDto(Cart cart)
        {
            var cartDto = new CartToReturnDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(item => new CartItemToReturnDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ImageUrl = item.Product.ImageUrl ?? string.Empty,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                    Total = item.Quantity * item.Product.Price
                }).ToList()
            };

            cartDto.Subtotal = cartDto.Items.Sum(i => i.Total);
            return cartDto;
        }
    }
}
