using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Entities.OrderAggregate;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Services.Dtos.OrderDtos;
using Pharmacy.Domain.Specification;

namespace Pharmacy.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IAreaShippingFeeService _shippingService;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Address> _addressRepo;
        private readonly IEmailService _emailService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartService cartService,
            IAreaShippingFeeService shippingService,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Address> addressRepo,
            IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _cartService = cartService;
            _shippingService = shippingService;
            _productRepo = productRepo;
            _addressRepo = addressRepo;
            _emailService = emailService;
        }

        public async Task<OrderToReturnDto> CreateOrderAsync(string buyerId, string buyerName, CreateOrderDto dto)
        {
            // 1. Get Cart
            var cart = await _cartService.GetCartAsync(dto.CartId, buyerId);
            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart not found or empty.");

            if (dto.ShippingAddress == null || string.IsNullOrWhiteSpace(dto.ShippingAddress.Area))
                throw new Exception("Shipping Address with Area is required.");

            // 2. Get or Create Selected Address
            var addresses = await _addressRepo.GetAllAsync();
            var address = addresses.FirstOrDefault(a => a.AppUserId == buyerId);

            if (address == null)
            {
                address = new Address
                {
                    AppUserId = buyerId,
                    Street = dto.ShippingAddress.Street,
                    Area = dto.ShippingAddress.Area,
                    City = string.IsNullOrWhiteSpace(dto.ShippingAddress.City) ? "Hurghada" : dto.ShippingAddress.City,
                    Country = string.IsNullOrWhiteSpace(dto.ShippingAddress.Country) ? "Egypt" : dto.ShippingAddress.Country
                };
                await _addressRepo.AddAsync(address);
            }
            else
            {
                address.Street = dto.ShippingAddress.Street;
                address.Area = dto.ShippingAddress.Area;
                address.City = string.IsNullOrWhiteSpace(dto.ShippingAddress.City) ? "Hurghada" : dto.ShippingAddress.City;
                address.Country = string.IsNullOrWhiteSpace(dto.ShippingAddress.Country) ? "Egypt" : dto.ShippingAddress.Country;
                _addressRepo.Update(address);
            }

            if (string.IsNullOrWhiteSpace(dto.BuyerEmail) || string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new Exception("Email and Phone Number are required.");

            // 3. Resolve Shipping Fee
            var shippingFee = await _shippingService.ResolveFeeByAreaAsync(address.Area);

            var orderItems = new List<Domain.Entities.OrderAggregate.OrderItem>();
            decimal subtotal = 0;

            // 4. Validate and Reduce Stock
            foreach (var item in cart.Items)
            {
                var product = await _productRepo.GetAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found.");

                if (product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for {product.Name}. Available: {product.Stock}");

                product.Stock -= item.Quantity;
                _productRepo.Update(product);
                
                var itemOrdered = new ProductItemOrdered(product.Id, product.Name, product.ImageUrl);
                var orderItem = new Domain.Entities.OrderAggregate.OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = product.Price, // Use DB price, not cart price
                    Quantity = item.Quantity
                };

                orderItems.Add(orderItem);
                subtotal += product.Price * item.Quantity;
            }

            var shipToAddress = new OrderAddress
            {
                Street = address.Street,
                Area = address.Area,
                City = address.City,
                Country = address.Country
            };

            var order = new Order
            {
                BuyerId = buyerId,
                BuyerName = buyerName,
                BuyerEmail = dto.BuyerEmail,
                
                PhoneNumber = dto.PhoneNumber,
                ShipToAddress = shipToAddress,
                OrderItems = orderItems,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Status = OrderStatus.Pending
            };

            await _orderRepository.AddAsync(order);
            
            // This implicitly saves the order and the product stock updates since they share DbContext inside DI boundary
            await _orderRepository.SaveChangesAsync();

            // 5. Clear Cart on success
            await _cartService.ClearCartAsync(dto.CartId, buyerId);

            var orderDto = MapOrderToDto(order);

            // 6. Send email notifications (fire-and-forget, don't block the response)
            _ = _emailService.SendOrderCreatedToAdminAsync(orderDto);
            _ = _emailService.SendOrderCreatedToUserAsync(orderDto);

            return orderDto;
        }

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(int orderId, string buyerId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId, buyerId);
            if (order == null) return null;

            return MapOrderToDto(order);
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string buyerId)
        {
            var orders = await _orderRepository.GetOrdersForUserAsync(buyerId);
            return orders.Select(MapOrderToDto).ToList();
        }

        // Admin Methods
        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderToReturnDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            return MapOrderToDto(order);
        }

        public async Task<OrderToReturnDto?> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            if (Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            {
                // If transitioning to Cancelled from any other state, restore stock
                if (newStatus == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var product = await _productRepo.GetAsync(item.ItemOrdered.ProductItemId);
                        if (product != null)
                        {
                            product.Stock += item.Quantity;
                            _productRepo.Update(product);
                        }
                    }
                }

                order.Status = newStatus;
                await _orderRepository.SaveChangesAsync();

                var orderDto = MapOrderToDto(order);

                // Send email notifications based on the new status
                switch (newStatus)
                {
                    case OrderStatus.Confirmed:
                        _ = _emailService.SendOrderConfirmedToUserAsync(orderDto);
                        break;
                    case OrderStatus.Shipped:
                        _ = _emailService.SendOrderShippedToUserAsync(orderDto);
                        break;
                    case OrderStatus.Delivered:
                        _ = _emailService.SendOrderDeliveredToUserAsync(orderDto);
                        break;
                    case OrderStatus.Cancelled:
                        _ = _emailService.SendOrderCancelledToUserAsync(orderDto);
                        break;
                }

                return orderDto;
            }
            else
            {
                throw new Exception($"Invalid Order Status provided: {status}");
            }
        }

        private OrderToReturnDto MapOrderToDto(Order order)
        {
            return new OrderToReturnDto
            {
                Id = order.Id,
                BuyerId = order.BuyerId,
                BuyerName = order.BuyerName,
                BuyerEmail = order.BuyerEmail,

                PhoneNumber = order.PhoneNumber,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                ShippingFee = order.ShippingFee,
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                ShipToAddress = new OrderAddressDto
                {
                    Street = order.ShipToAddress.Street,
                    Area = order.ShipToAddress.Area,
                    City = order.ShipToAddress.City,
                    Country = order.ShipToAddress.Country
                },
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ItemOrdered.ProductItemId,
                    ProductName = i.ItemOrdered.ProductName,
                    PictureUrl = i.ItemOrdered.PictureUrl,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
