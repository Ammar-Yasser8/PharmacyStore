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

        public async Task<CartToReturnDto?> GetCartAsync(string cartId, string? userId = null)
        {
            var cart = await ResolveCartAsync(cartId, userId);
            await _cartRepository.SaveChangesAsync();

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
            await ResolveCartAsync(cartId, userId);
            await _cartRepository.SaveChangesAsync();
        }

        private async Task<Cart> ResolveCartAsync(string cartId, string? userId)
        {
            Cart? userCart = null;
            if (!string.IsNullOrEmpty(userId))
            {
                userCart = await _cartRepository.GetCartByUserIdAsync(userId);
            }

            var currentCart = await _cartRepository.GetCartAsync(cartId);

            if (userCart != null)
            {
                // If user already has an official cart, and it's different from the current cartId, 
                // we should merge the items and use the official cart.
                if (currentCart != null && currentCart.Id != userCart.Id)
                {
                    foreach (var item in currentCart.Items)
                    {
                        var targetItem = userCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                        if (targetItem != null)
                        {
                            targetItem.Quantity += item.Quantity;
                        }
                        else
                        {
                            userCart.Items.Add(new CartItem
                            {
                                CartId = userCart.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity
                            });
                        }
                    }
                    // The anonymous cart items are now merged. 
                    // We don't delete the anonymous cart here to avoid complex state management, 
                    // but we ensure the user is redirected to their official cart.
                }
                return userCart;
            }

            if (currentCart == null)
            {
                // Neither exists, create a new one.
                currentCart = new Cart { Id = cartId, AppUserId = userId };
                await _cartRepository.AddCartAsync(currentCart);
            }
            else if (!string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(currentCart.AppUserId))
            {
                // Assign anonymous cart to user if they don't have one yet.
                currentCart.AppUserId = userId;
            }

            return currentCart;
        }

        public async Task<CartToReturnDto?> AddItemAsync(string cartId, int productId, int quantity, string? userId = null)
        {
            if (quantity <= 0) return null;

            var cart = await ResolveCartAsync(cartId, userId);

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
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _cartRepository.SaveChangesAsync();

            // Refresh cart to ensure we return the latest state (with user official cart Id if redirected)
            return await GetCartAsync(cart.Id, userId);
        }

        public async Task<CartToReturnDto?> UpdateItemQuantityAsync(string cartId, int productId, int quantity, string? userId = null)
        {
            var cart = await ResolveCartAsync(cartId, userId);

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

            return await GetCartAsync(cart.Id, userId);
        }

        public async Task<CartToReturnDto?> RemoveItemAsync(string cartId, int productId, string? userId = null)
        {
            var cart = await ResolveCartAsync(cartId, userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                cart.Items.Remove(existingItem);
                await _cartRepository.SaveChangesAsync();
            }

            return await GetCartAsync(cart.Id, userId);
        }

        public async Task<CartToReturnDto?> ClearCartAsync(string cartId, string? userId = null)
        {
            var cart = await ResolveCartAsync(cartId, userId);

            cart.Items.Clear();
            await _cartRepository.SaveChangesAsync();

            return await GetCartAsync(cart.Id, userId);
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
