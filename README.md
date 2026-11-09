# Foodeaz ASP.NET Core migration (starter conversion)

This solution migrates the supplied PHP website into an ASP.NET Core Razor Pages project and carries across the site's image, CSS, and JavaScript assets. Public PHP page markup has been retained where possible, with PHP execution blocks removed; PHP-specific loops/data rendering may therefore need manual completion and visual QA before production. This is a migration baseline, not a claim of exact feature parity.

## Requirements
- .NET 8 SDK

## Run
```bash
cd Foodeaz.Web
dotnet restore
dotnet run
```
Open the URL printed by `dotnet run`.

## SMTP configuration for Contact form
Set these environment variables on the host (do not commit credentials):
- `Smtp__Host` (e.g. smtp.gmail.com)
- `Smtp__Port` (587)
- `Smtp__EnableSsl` (true)
- `Smtp__Username`
- `Smtp__Password` (use an app password / secret store)
- `Smtp__FromEmail`
- `Smtp__ToEmail`

The credentials present in the original PHP project are intentionally not copied. Rotate any credential that was embedded in source code.

## Notes / next steps
- Verify each route and asset path after deployment.
- Reimplement any removed PHP data loops and dynamic behavior in C# page models.
- The original PHP contact handler has been replaced by a validated Razor Pages form.
- No database was configured because the supplied pages did not establish a clear application database contract.
