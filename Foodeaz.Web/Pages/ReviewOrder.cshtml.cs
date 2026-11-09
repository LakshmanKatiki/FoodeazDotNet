using System.Text.Json;
using Foodeaz.Web.Data;
using Foodeaz.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodeaz.Web.Pages;

public class ReviewOrderModel : PageModel
{
    private const string FormKey = "FoodeazPurchaseForm";
    private readonly OrderService _orders;
    private readonly FoodeazDbContext _db;
    private readonly RazorpayService _razorpay;

    public ReviewOrderModel(OrderService orders, FoodeazDbContext db, RazorpayService razorpay)
    {
        _orders = orders;
        _db = db;
        _razorpay = razorpay;
    }

    public PurchaseModel.PurchaseFormData? Order { get; private set; }
    public List<PurchaseModel.CartLine> Items { get; private set; } = new();
    public decimal Subtotal => Items.Sum(i => i.Price * i.Quantity);
    public decimal DeliveryCharge => Items.Count > 0 ? OrderService.DeliveryCharge : 0m;
    public decimal Total => Subtotal + DeliveryCharge;

    public IActionResult OnGet()
    {
        var json = TempData.Peek(FormKey) as string;
        if (string.IsNullOrWhiteSpace(json)) return RedirectToPage("/Purchase");

        Order = JsonSerializer.Deserialize<PurchaseModel.PurchaseFormData>(json);
        if (Order is null) return RedirectToPage("/Purchase");

        LoadItems();
        if (Items.Count == 0) return RedirectToPage("/Purchase");
        TempData.Keep(FormKey);
        return Page();
    }

    public async Task<IActionResult> OnPostPayAsync(CancellationToken ct)
    {
        var json = TempData.Peek(FormKey) as string;
        if (string.IsNullOrWhiteSpace(json)) return RedirectToPage("/Purchase");

        Order = JsonSerializer.Deserialize<PurchaseModel.PurchaseFormData>(json);
        if (Order is null || Order.DeliveryDate.Date < DateTime.Today.AddDays(1))
            return RedirectToPage("/Purchase");

        LoadItems();
        if (Items.Count == 0) return RedirectToPage("/Purchase");

        // Save the order in the database before creating the Razorpay checkout order.
        var saved = await _orders.SaveOrderBeforePaymentAsync(
            Order.CustomerName ?? "", Order.Email ?? "", Order.Mobile ?? "", Order.Address ?? "",
            Order.DeliveryDate, Order.Comments,
            Items.Select(i => (i.Id, i.Name, i.Price, i.Quantity)), ct);

        try
        {
            var gatewayOrder = await _razorpay.CreateOrderAsync(saved.OrderNumber, saved.Total, ct);
            saved.RazorpayOrderId = gatewayOrder.OrderId;
            await _db.SaveChangesAsync(ct);
            return RedirectToPage("/Payment", new { id = saved.Id });
        }
        catch
        {
            // The database record remains available for support/retry if gateway setup or API fails.
            return RedirectToPage("/PaymentFailed", new { id = saved.Id, setupError = true });
        }
    }

    private void LoadItems()
    {
        // Product details, image paths, and prices are controlled by the server.
        var catalogue = new List<PurchaseModel.CartLine>
        {
            new() { Id = "FZ001", Name = "Foodeaz Dosa Batter (1000 g)", Price = 97m, ImageUrl = "/img/dosa-batter.jpg" },
            new() { Id = "FZ002", Name = "Foodeaz Rava Idli Batter (1000 g)", Price = 97m, ImageUrl = "/img/rava-idli-batter.jpg" },
            new() { Id = "FZ003", Name = "Foodeaz Aata Tawa Roti (500 g / 12 rotis)", Price = 77m, ImageUrl = "/img/aata-tawa-roti.jpg" },
            new() { Id = "FZ004", Name = "Foodeaz Malabar Parota (400 g / 5 parotas)", Price = 97m, ImageUrl = "/img/malabar-parota.jpg" }
        };

        Items = catalogue
            .Where(p => Order!.Quantities.TryGetValue(p.Id, out var qty) && qty > 0)
            .Select(p =>
            {
                p.Quantity = Math.Clamp(Order!.Quantities[p.Id], 0, 99);
                return p;
            })
            .ToList();
    }
}
