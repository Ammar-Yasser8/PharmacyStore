namespace Pharmacy.Domain.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        PaymentFailed,
        Shipped,
        Delivered,
        Cancelled
    }
}
