# Foodeaz SQL Server payment patch (overlay only)

## Apply
1. Back up your existing Foodeaz project.
2. Copy the files in this ZIP into the matching paths under `FoodeazDotNet/Foodeaz.Web/`, allowing replacement. These are overlay files only; unrelated pages/assets are not included.
3. Merge `appsettings.payment.sample.json` into your real `appsettings.json`. Do not overwrite unrelated settings. Set `ConnectionStrings:DefaultConnection` to your SQL Server connection string. Keep passwords and Razorpay secrets out of source control; use environment variables or deployment secrets.
4. Restore/build the .NET 8 project.

## SQL Server tables
This patch maps orders to `dbo.FoodeazOrdersNew` and items to `dbo.FoodeazOrderItems`. The supplied schema already creates these tables and `dbo.FoodeazNotificationLogs`. The notification log table is not written by this patch; notification delivery is attempted by `NotificationService` and errors are logged by the application.

`Program.cs` intentionally does not call `EnsureCreated()` because your tables already exist. The model maps `OrderId`, `DeliveryAddress`, `CustomerComments`, `SubTotal`, `GrandTotal`, `OrderStatus`, `GatewayOrderId`, `CreatedAt`, and `PaidAt` to the C# model properties. Order status remains `AwaitingPayment` on payment failure because your SQL check constraint does not permit `PaymentFailed`; payment status is set to `Failed`.

## Connection string
Use the key `DefaultConnection`. The sample file intentionally contains placeholders, not your database password. Since a database `sa` password was shared in chat, rotate it if it is a real credential and create a dedicated least-privilege application login. Only enable `TrustServerCertificate=True` if appropriate for your hosting environment; prefer a valid trusted SQL Server certificate in production.

## Razorpay and notifications
Set `Razorpay__KeyId`, `Razorpay__KeySecret`, SMTP settings, and the WhatsApp webhook settings through environment variables or secret storage. Test with Razorpay test keys first. The WhatsApp webhook is generic and may need adaptation to your provider's required API payload. Configure Razorpay webhooks for production reconciliation.
