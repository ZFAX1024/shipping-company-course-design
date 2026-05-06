using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Data;

public sealed class ShippingDbContext : DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<ShippingRoute> ShippingRoutes => Set<ShippingRoute>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Cargo> Cargoes => Set<Cargo>();
    public DbSet<TransportOrder> TransportOrders => Set<TransportOrder>();
    public DbSet<FinanceSettlement> FinanceSettlements => Set<FinanceSettlement>();
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.UserName).IsUnique();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<Vessel>(entity =>
        {
            entity.HasIndex(x => x.ImoNumber).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ImoNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Type).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CurrentPort).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CapacityTons).HasPrecision(18, 2);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<ShippingRoute>(entity =>
        {
            entity.HasIndex(x => x.RouteCode).IsUnique();
            entity.Property(x => x.RouteCode).HasMaxLength(50).IsRequired();
            entity.Property(x => x.OriginPort).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DestinationPort).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DistanceNm).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ContactName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(120);
            entity.Property(x => x.Address).HasMaxLength(240);
            entity.Property(x => x.CreditCode).HasMaxLength(80);
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(80).IsRequired();
            entity.Property(x => x.WeightTons).HasPrecision(18, 2);
            entity.Property(x => x.VolumeCbm).HasPrecision(18, 2);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Cargoes)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TransportOrder>(entity =>
        {
            entity.HasIndex(x => x.OrderNo).IsUnique();
            entity.Property(x => x.OrderNo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.FreightAmount).HasPrecision(18, 2);
            entity.Property(x => x.Remarks).HasMaxLength(500);
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Cargo)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CargoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Vessel)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.VesselId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.ShippingRoute)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ShippingRouteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FinanceSettlement>(entity =>
        {
            entity.HasIndex(x => x.SettlementNo).IsUnique();
            entity.HasIndex(x => x.TransportOrderId).IsUnique();
            entity.Property(x => x.SettlementNo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ReceivableAmount).HasPrecision(18, 2);
            entity.Property(x => x.PaidAmount).HasPrecision(18, 2);
            entity.Property(x => x.TaxAmount).HasPrecision(18, 2);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasOne(x => x.TransportOrder)
                .WithOne(x => x.Settlement)
                .HasForeignKey<FinanceSettlement>(x => x.TransportOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
            entity.Property(x => x.TransactionNo).HasMaxLength(80);
            entity.Property(x => x.Notes).HasMaxLength(300);
            entity.HasOne(x => x.FinanceSettlement)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.FinanceSettlementId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
