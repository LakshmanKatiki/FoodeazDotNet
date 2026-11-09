using System.ComponentModel.DataAnnotations;

namespace Foodeaz.Web.Models;

public class CustomerOrder
{
    public long Id { get; set; }
    [MaxLength(30)] public string OrderNumber { get; set; } = "";
    [MaxLength(150)] public string CustomerName { get; set; } = "";
    [MaxLength(254)] public string? Email { get; set; }
    [MaxLength(25)] public string Mobile { get; set; } = "";
    [MaxLength(500)] public string Address { get; set; } = "";
    public DateTime DeliveryDate { get; set; }
    [MaxLength(1000)] public string? Comments { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DeliveryCharge { get; set; } = 49m;
    public decimal Total { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "AwaitingPayment";
    [MaxLength(100)] public string? RazorpayOrderId { get; set; }
    [MaxLength(100)] public string? TransactionId { get; set; }
    [MaxLength(20)] public string PaymentStatus { get; set; } = "Pending";
    public DateTime CreatedUtc { get; set; }
    public DateTime? PaidUtc { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
