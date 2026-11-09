# Foodeaz payment patch (overlay only)

Copy the files in `FoodeazDotNet/Foodeaz.Web/` over the same paths in your existing project. Existing unrelated pages and assets are not included. Do not replace your existing `appsettings.json`; merge the settings from `appsettings.payment.sample.json` into it.

## What this patch does
- Adds a flat ₹49 delivery charge to every non-empty order.
- Saves the order and order items in SQLite before requesting a Razorpay checkout order.
- Creates Razorpay orders using server-side credentials and verifies the payment signature server-side.
- Updates `TransactionId`, `PaymentStatus=Paid`, and `Status=Confirmed` on the same database order after signature verification.
- Includes payment success/failure pages and notification hooks for email (SMTP) and WhatsApp (configured webhook).

## Configure credentials securely
Set these values through environment variables or user-secrets in production; do not commit real secrets to `appsettings.json`.

Razorpay:
- `Razorpay__KeyId`
- `Razorpay__KeySecret`

SMTP:
- `Smtp__Host`
- `Smtp__Port` (usually 587)
- `Smtp__EnableSsl` (true)
- `Smtp__Username`
- `Smtp__Password`
- `Smtp__FromEmail`

WhatsApp:
- `WhatsApp__WebhookUrl`
- `WhatsApp__BearerToken` (if your webhook requires it)

The WhatsApp hook sends a generic JSON POST with `to`, `orderNumber`, `status`, `amount`, `transactionId`, and `message`. Your WhatsApp provider may require a provider-specific payload/template; adapt `Services/NotificationService.cs` to that provider's API. Notifications are skipped if not configured and errors are logged; they do not undo payment confirmation.

## Database
This patch uses SQLite (`Data Source=foodeaz.db`) and calls `EnsureCreated()` at startup. Back up any existing database before deployment. `EnsureCreated()` does not migrate an existing schema. If you already have a production database/schema, create and apply an EF Core migration or map these entities to your existing tables before deploying.

## Important operational notes
- Use Razorpay test-mode keys and test payments first.
- Configure HTTPS and a real public callback URL before production.
- For reliable reconciliation when a customer closes the browser after payment, configure Razorpay webhooks on your server and process `payment.captured` / `order.paid` idempotently. The current browser callback verifies the signature but a webhook is recommended for production.
- If gateway order creation fails, the order remains saved with pending payment status for support/retry.
- Do not treat a client-side failure/close event as definitive proof that Razorpay did not collect money; reconcile with Razorpay dashboard/webhooks before refunding or cancelling.
- I could not build/test this project in this environment because the .NET SDK is unavailable.
