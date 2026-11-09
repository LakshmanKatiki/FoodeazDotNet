using Foodeaz.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Foodeaz.Web.Data;

public class FoodeazDbContext(DbContextOptions<FoodeazDbContext> options) : DbContext(options)
{
    public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var order = modelBuilder.Entity<CustomerOrder>();
        order.ToTable("FoodeazOrdersNew", "dbo");
        order.HasKey(x => x.Id);
        order.Property(x => x.Id).HasColumnName("OrderId").UseIdentityColumn();
        order.Property(x => x.OrderNumber).HasMaxLength(30).IsRequired();
        order.HasIndex(x => x.OrderNumber).IsUnique();
        order.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
        order.Property(x => x.Email).HasMaxLength(254).IsRequired(false);
        order.Property(x => x.Mobile).HasMaxLength(25).IsRequired();
        order.Property(x => x.Address).HasColumnName("DeliveryAddress").HasMaxLength(500).IsRequired();
        order.Property(x => x.DeliveryDate).HasColumnType("date");
        order.Property(x => x.Comments).HasColumnName("CustomerComments").HasMaxLength(1000).IsRequired(false);
        order.Property(x => x.Subtotal).HasColumnName("SubTotal").HasPrecision(12, 2);
        order.Property(x => x.DeliveryCharge).HasPrecision(12, 2).HasDefaultValue(49m);
        order.Property(x => x.Total).HasColumnName("GrandTotal").HasPrecision(12, 2);
        order.Property(x => x.Status).HasColumnName("OrderStatus").HasMaxLength(30).HasDefaultValue("AwaitingPayment");
        order.Property(x => x.RazorpayOrderId).HasColumnName("GatewayOrderId").HasMaxLength(100).IsRequired(false);
        order.Property(x => x.TransactionId).HasMaxLength(100).IsRequired(false);
        order.Property(x => x.PaymentStatus).HasMaxLength(20).HasDefaultValue("Pending");
        order.Property(x => x.CreatedUtc).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAdd();
        order.Property(x => x.PaidUtc).HasColumnName("PaidAt").HasColumnType("datetime2(0)").IsRequired(false);

        var item = modelBuilder.Entity<OrderItem>();
        item.ToTable("FoodeazOrderItems", "dbo");
        item.HasKey(x => x.Id);
        item.Property(x => x.Id).HasColumnName("OrderItemId").UseIdentityColumn();
        item.Property(x => x.CustomerOrderId).HasColumnName("OrderId");
        item.Property(x => x.ProductId).HasMaxLength(50).IsRequired(false);
        item.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        item.Property(x => x.UnitPrice).HasPrecision(12, 2);
        item.Property(x => x.LineTotal).HasPrecision(12, 2);
        item.Property<DateTime>("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()").ValueGeneratedOnAdd();
        item.HasIndex(x => x.CustomerOrderId);
        order.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.CustomerOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
