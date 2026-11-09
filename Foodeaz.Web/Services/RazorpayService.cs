using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Foodeaz.Web.Services;

public class RazorpayService(HttpClient http, IConfiguration configuration)
{
    private string KeyId => configuration["Razorpay:KeyId"] ?? "";
    private string Secret => configuration["Razorpay:KeySecret"] ?? "";
    public string PublicKey => KeyId;

    public async Task<RazorpayOrderResult> CreateOrderAsync(string receipt, decimal amount, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(KeyId) || string.IsNullOrWhiteSpace(Secret))
            throw new InvalidOperationException("Razorpay KeyId and KeySecret must be configured.");
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.razorpay.com/v1/orders");
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{KeyId}:{Secret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero),
            currency = configuration["Razorpay:Currency"] ?? "INR",
            receipt,
            notes = new { source = "Foodeaz" }
        }), Encoding.UTF8, "application/json");
        using var response = await http.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Razorpay order creation failed ({(int)response.StatusCode}).");
        using var doc = JsonDocument.Parse(body);
        return new RazorpayOrderResult(doc.RootElement.GetProperty("id").GetString()!, KeyId);
    }

    public bool VerifySignature(string razorpayOrderId, string paymentId, string signature)
    {
        if (string.IsNullOrWhiteSpace(Secret) || string.IsNullOrWhiteSpace(signature)) return false;
        var payload = Encoding.UTF8.GetBytes($"{razorpayOrderId}|{paymentId}");
        var key = Encoding.UTF8.GetBytes(Secret);
        var hash = HMACSHA256.HashData(key, payload);
        byte[] supplied;
        try { supplied = Convert.FromHexString(signature); } catch { return false; }
        return supplied.Length == hash.Length && CryptographicOperations.FixedTimeEquals(hash, supplied);
    }
}
public record RazorpayOrderResult(string OrderId, string KeyId);
