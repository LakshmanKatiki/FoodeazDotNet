using System.Net;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Foodeaz.Web.Models;

namespace Foodeaz.Web.Services;

public class NotificationService(IConfiguration config, IHttpClientFactory factory, ILogger<NotificationService> logger)
{
    public async Task SendPaymentResultAsync(CustomerOrder order, bool success, CancellationToken ct = default)
    {
        var status = success ? "Payment successful" : "Payment failed";
        var body = $"Foodeaz Order: {order.OrderNumber}\nStatus: {status}\nAmount: ₹{order.Total:0.00}\nTransaction ID: {(string.IsNullOrWhiteSpace(order.TransactionId) ? "Not available" : order.TransactionId)}\nDelivery date: {order.DeliveryDate:dd MMM yyyy}";
        if (!string.IsNullOrWhiteSpace(order.Email))
        {
            try { await SendEmailAsync(order.Email, $"Foodeaz order {order.OrderNumber}: {status}", body, ct); }
            catch (Exception ex) { logger.LogError(ex, "Email notification failed for order {OrderNumber}", order.OrderNumber); }
        }
        try { await SendWhatsAppAsync(order, success, body, ct); }
        catch (Exception ex) { logger.LogError(ex, "WhatsApp notification failed for order {OrderNumber}", order.OrderNumber); }
    }

    private async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct)
    {
        var host = config["Smtp:Host"];
        var from = config["Smtp:FromEmail"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) ||
            string.IsNullOrWhiteSpace(config["Smtp:Username"]) || string.IsNullOrWhiteSpace(config["Smtp:Password"]))
        { logger.LogWarning("SMTP is not configured; email notification skipped."); return; }

        using var message = new MailMessage(from, to, subject, body);
        using var client = new SmtpClient(host, int.TryParse(config["Smtp:Port"], out var port) ? port : 587)
        {
            EnableSsl = bool.TryParse(config["Smtp:EnableSsl"], out var ssl) && ssl,
            Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"])
        };
        await client.SendMailAsync(message, ct);
    }

    private async Task SendWhatsAppAsync(CustomerOrder order, bool success, string body, CancellationToken ct)
    {
        var url = config["WhatsApp:WebhookUrl"];
        if (string.IsNullOrWhiteSpace(url)) { logger.LogWarning("WhatsApp webhook is not configured; WhatsApp notification skipped."); return; }
        var client = factory.CreateClient();
        var token = config["WhatsApp:BearerToken"];
        if (!string.IsNullOrWhiteSpace(token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(new
        {
            to = order.Mobile,
            orderNumber = order.OrderNumber,
            status = success ? "success" : "failed",
            amount = order.Total,
            transactionId = order.TransactionId,
            message = body
        }), Encoding.UTF8, "application/json"), ct);
        if (!response.IsSuccessStatusCode) logger.LogWarning("WhatsApp webhook returned HTTP {StatusCode}.", (int)response.StatusCode);
    }
}
