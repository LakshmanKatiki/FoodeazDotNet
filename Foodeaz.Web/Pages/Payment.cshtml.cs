using Foodeaz.Web.Data;
using Foodeaz.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Foodeaz.Web.Pages;

public class PaymentModel : PageModel
{
    private readonly FoodeazDbContext _db;
    private readonly RazorpayService _razorpay;
    private readonly NotificationService _notifications;
    public PaymentModel(FoodeazDbContext db, RazorpayService razorpay, NotificationService notifications)
    { _db = db; _razorpay = razorpay; _notifications = notifications; }

    public Foodeaz.Web.Models.CustomerOrder? Order { get; private set; }
    public string KeyId => _razorpay.PublicKey;

    public async Task<IActionResult> OnGetAsync(long id, CancellationToken ct)
    {
        Order = await _db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (Order is null) return NotFound();
        if (Order.PaymentStatus == "Paid") return RedirectToPage("/PaymentSuccess", new { id });
        if (string.IsNullOrWhiteSpace(Order.RazorpayOrderId)) return RedirectToPage("/PaymentFailed", new { id, setupError = true });
        return Page();
    }

    public async Task<IActionResult> OnPostSuccessAsync(int id, string razorpay_payment_id, string razorpay_order_id, string razorpay_signature, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null) return NotFound();
        if (order.PaymentStatus == "Paid") return RedirectToPage("/PaymentSuccess", new { id });
        if (!string.Equals(order.RazorpayOrderId, razorpay_order_id, StringComparison.Ordinal) ||
            !_razorpay.VerifySignature(razorpay_order_id, razorpay_payment_id, razorpay_signature))
        {
            order.PaymentStatus = "Failed";
            order.Status = "AwaitingPayment";
            await _db.SaveChangesAsync(ct);
            await _notifications.SendPaymentResultAsync(order, false, ct);
            return RedirectToPage("/PaymentFailed", new { id, verificationError = true });
        }

        // Update transaction ID on the same saved order, only after server-side signature verification.
        order.TransactionId = razorpay_payment_id;
        order.PaymentStatus = "Paid";
        order.Status = "Confirmed";
        order.PaidUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        await _notifications.SendPaymentResultAsync(order, true, ct);
        return RedirectToPage("/PaymentSuccess", new { id });
    }

    public async Task<IActionResult> OnPostFailureAsync(long id, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null) return NotFound();
        if (order.PaymentStatus != "Paid")
        {
            order.PaymentStatus = "Failed";
            order.Status = "AwaitingPayment";
            await _db.SaveChangesAsync(ct);
            await _notifications.SendPaymentResultAsync(order, false, ct);
        }
        return RedirectToPage("/PaymentFailed", new { id });
    }
}
