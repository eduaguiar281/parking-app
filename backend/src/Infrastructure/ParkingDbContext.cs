using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Infrastructure;

public sealed class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<Spot> Spots => Set<Spot>();
    public DbSet<TariffTable> Tariffs => Set<TariffTable>();
    public DbSet<ParkingStay> Stays => Set<ParkingStay>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Login).IsUnique();
            e.Property(x => x.Login).HasMaxLength(120);
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.Role).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
        });

        modelBuilder.Entity<Sector>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.AllowedCategories).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
        });

        modelBuilder.Entity<Spot>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.VehicleCategory).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
        });

        modelBuilder.Entity<TariffTable>(e =>
        {
            e.Property(x => x.VehicleType).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.FirstHourAmount).HasPrecision(12, 2);
            e.Property(x => x.AdditionalHourAmount).HasPrecision(12, 2);
        });

        modelBuilder.Entity<ParkingStay>(e =>
        {
            e.Property(x => x.VehicleType).HasConversion<string>();
            e.Property(x => x.PaymentMethod).HasConversion<string>();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.FirstHourAmountSnapshot).HasPrecision(12, 2);
            e.Property(x => x.AdditionalHourAmountSnapshot).HasPrecision(12, 2);
            e.Property(x => x.AmountCharged).HasPrecision(12, 2);
            e.HasIndex(x => x.Plate)
                .IsUnique()
                .HasFilter("Status = 'Active'");
            e.HasIndex(x => x.SpotId)
                .IsUnique()
                .HasFilter("Status = 'Active'");
        });

        modelBuilder.Entity<CashRegister>(e =>
        {
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.OpeningAmount).HasPrecision(12, 2);
            e.Property(x => x.InformedClosingAmount).HasPrecision(12, 2);
            e.Property(x => x.ExpectedAmount).HasPrecision(12, 2);
            e.Property(x => x.Difference).HasPrecision(12, 2);
            e.HasIndex(x => x.OperationalDate).IsUnique();
            e.HasIndex(x => x.Status)
                .IsUnique()
                .HasFilter("Status = 'Open'");
        });

        modelBuilder.Entity<CashMovement>(e =>
        {
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.PaymentMethod).HasConversion<string>();
            e.Property(x => x.Amount).HasPrecision(12, 2);
        });
    }
}
