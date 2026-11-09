using Foodeaz.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Foodeaz.Web.Pages;

public class PaymentSuccessModel(FoodeazDbContext db) : PageModel
{
    public Foodeaz.Web.Models.CustomerOrder? Order { get; private set; }
    public async Task<IActionResult> OnGetAsync(long id, CancellationToken ct)
    {
        Order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (Order is null) return NotFound();
        if (Order.PaymentStatus != "Paid") return RedirectToPage("/PaymentFailed", new { id });
        return Page();
    }
}
