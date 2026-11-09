using Foodeaz.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Foodeaz.Web.Pages;

public class PaymentFailedModel(FoodeazDbContext db) : PageModel
{
    public Foodeaz.Web.Models.CustomerOrder? Order { get; private set; }
    [BindProperty(SupportsGet = true)] public bool SetupError { get; set; }
    [BindProperty(SupportsGet = true)] public bool VerificationError { get; set; }
    public async Task<IActionResult> OnGetAsync(long id, CancellationToken ct)
    {
        Order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (Order is null) return NotFound();
        if (Order.PaymentStatus == "Paid") return RedirectToPage("/PaymentSuccess", new { id });
        return Page();
    }
}
