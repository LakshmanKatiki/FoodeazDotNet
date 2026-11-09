using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodeaz.Web.Pages;

public class PurchaseModel : PageModel
{
    private const string FormKey = "FoodeazPurchaseForm";
    private const decimal DeliveryCharge = 49m;

    [BindProperty, Required, Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;

    [BindProperty, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
    public string Mobile { get; set; } = string.Empty;

    [BindProperty, Required, Display(Name = "Delivery Address")]
    public string Address { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Date), Display(Name = "Preferred Delivery Date")]
    public DateTime? DeliveryDate { get; set; }

    [BindProperty, Display(Name = "Delivery Instructions (optional)")]
    public string? Comments { get; set; }

    [BindProperty]
    public List<CartLine> Products { get; set; } = new();

    public DateTime EarliestDeliveryDate => DateTime.Today.AddDays(1);
    public decimal Subtotal => Products.Where(p => p.Quantity > 0).Sum(p => p.Price * p.Quantity);
    public decimal Delivery => Subtotal > 0 ? DeliveryCharge : 0m;
    public decimal Total => Subtotal + Delivery;

    public void OnGet()
    {
        LoadCatalogue();
        var json = TempData.Peek(FormKey) as string;
        if (string.IsNullOrWhiteSpace(json))
        {
            DeliveryDate = EarliestDeliveryDate;
            return;
        }

        var saved = JsonSerializer.Deserialize<PurchaseFormData>(json);
        if (saved is null)
        {
            DeliveryDate = EarliestDeliveryDate;
            return;
        }

        CustomerName = saved.CustomerName ?? string.Empty;
        Email = saved.Email ?? string.Empty;
        Mobile = saved.Mobile ?? string.Empty;
        Address = saved.Address ?? string.Empty;
        Comments = saved.Comments;
        DeliveryDate = saved.DeliveryDate < EarliestDeliveryDate ? EarliestDeliveryDate : saved.DeliveryDate;

        foreach (var product in Products)
        {
            if (saved.Quantities.TryGetValue(product.Id, out var quantity))
                product.Quantity = Math.Max(0, quantity);
        }
    }

    public IActionResult OnPostReview()
    {
        LoadCatalogue();

        // Accept only quantities from the posted cart. Names, prices, and image paths come from the server catalogue.
        var postedQuantities = (Products ?? new List<CartLine>())
            .GroupBy(p => p.Id)
            .ToDictionary(g => g.Key, g => Math.Max(0, g.First().Quantity));

        foreach (var product in Products)
            product.Quantity = postedQuantities.TryGetValue(product.Id, out var qty) ? qty : 0;

        if (DeliveryDate is null || DeliveryDate.Value.Date < EarliestDeliveryDate)
            ModelState.AddModelError(nameof(DeliveryDate), "Same-day delivery is not available. Please select tomorrow or a later date.");

        if (!Products.Any(p => p.Quantity > 0))
            ModelState.AddModelError(string.Empty, "Please select at least one product.");

        if (!ModelState.IsValid)
            return Page();

        var form = new PurchaseFormData
        {
            CustomerName = CustomerName,
            Email = Email,
            Mobile = Mobile,
            Address = Address,
            DeliveryDate = DeliveryDate!.Value.Date,
            Comments = Comments,
            Quantities = Products.ToDictionary(p => p.Id, p => p.Quantity)
        };

        TempData[FormKey] = JsonSerializer.Serialize(form);
        TempData.Keep(FormKey);
        return RedirectToPage("/ReviewOrder");
    }

    private void LoadCatalogue()
    {
        var oldQuantities = Products?.ToDictionary(p => p.Id, p => p.Quantity) ?? new Dictionary<string, int>();

        Products = new List<CartLine>
        {
            new() { Id = "FZ001", Name = "Foodeaz Dosa Batter (1000 g)", Price = 97m, ImageUrl = "/img/dosa-batter.jpg" },
            new() { Id = "FZ002", Name = "Foodeaz Rava Idli Batter (1000 g)", Price = 97m, ImageUrl = "/img/rava-idli-batter.jpg" },
            new() { Id = "FZ003", Name = "Foodeaz Aata Tawa Roti (500 g / 12 rotis)", Price = 77m, ImageUrl = "/img/aata-tawa-roti.jpg" },
            new() { Id = "FZ004", Name = "Foodeaz Malabar Parota (400 g / 5 parotas)", Price = 97m, ImageUrl = "/img/malabar-parota.jpg" }
        };

        foreach (var product in Products)
        {
            if (oldQuantities.TryGetValue(product.Id, out var qty))
                product.Quantity = Math.Max(0, qty);
        }
    }

    public class CartLine
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 99)]
        public int Quantity { get; set; }
    }

    public class PurchaseFormData
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? Comments { get; set; }
        public Dictionary<string, int> Quantities { get; set; } = new();
    }
}
