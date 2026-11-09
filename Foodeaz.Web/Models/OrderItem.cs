using System.ComponentModel.DataAnnotations;

namespace Foodeaz.Web.Models;

public class OrderItem
{
    public long Id { get; set; }
    public long CustomerOrderId { get; set; }
    public CustomerOrder? Order { get; set; }
    [MaxLength(50)] public string? ProductId { get; set; }
    [MaxLength(200)] public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
