using Foodeaz.Web.Data;
using Foodeaz.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// Database connection
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing.");
}

builder.Services.AddDbContext<FoodeazDbContext>(
    options => options.UseSqlServer(connectionString));

// HTTP clients
builder.Services.AddHttpClient<RazorpayService>();
builder.Services.AddHttpClient();

// Application services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// SQL Server tables already exist.
// Do not call EnsureCreated() here.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();
