using Foodeaz.Web.Data;
using Foodeaz.Web.Models;

namespace Foodeaz.Web.Services;

public class OrderService(FoodeazDbContext db)
{
    public const decimal DeliveryCharge = 49m;

    public async Task<CustomerOrder> SaveOrderBeforePaymentAsync(
        string name, string? email, string mobile, string address, DateTime deliveryDate, string? comments,
        IEnumerable<(string Id, string Name, decimal Price, int Quantity)> products,
        CancellationToken cancellationToken = default)
    {
        var selected = products.Where(p => p.Quantity > 0).ToList();
        if (selected.Count == 0) throw new InvalidOperationException("Select at least one product.");
        var subtotal = selected.Sum(p => p.Price * p.Quantity);
        var order = new CustomerOrder
        {
            OrderNumber = $"FZ{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            CustomerName = name.Trim(), Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(), Mobile = mobile.Trim(),
            Address = address.Trim(), DeliveryDate = deliveryDate.Date, Comments = string.IsNullOrWhiteSpace(comments) ? null : comments.Trim(),
            Subtotal = subtotal, DeliveryCharge = DeliveryCharge, Total = subtotal + DeliveryCharge,
            Status = "AwaitingPayment", PaymentStatus = "Pending",
            Items = selected.Select(p => new OrderItem
            {
                ProductId = p.Id, ProductName = p.Name, Quantity = p.Quantity,
                UnitPrice = p.Price, LineTotal = p.Price * p.Quantity
            }).ToList()
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        return order;
    }
}
