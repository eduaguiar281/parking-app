using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Infrastructure.Entities;

public sealed class ParkingStay
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public Guid SectorId { get; set; }
    public string SectorCodeSnapshot { get; set; } = string.Empty;
    public string SectorNameSnapshot { get; set; } = string.Empty;
    public Guid SpotId { get; set; }
    public string SpotCodeSnapshot { get; set; } = string.Empty;
    public Guid TariffTableId { get; set; }
    public string TariffNameSnapshot { get; set; } = string.Empty;
    public decimal FirstHourAmountSnapshot { get; set; }
    public decimal AdditionalHourAmountSnapshot { get; set; }
    public DateTimeOffset EntryAt { get; set; }
    public DateTimeOffset? ExitAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? AmountCharged { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public Guid? CashRegisterId { get; set; }
    public Guid EntryUserId { get; set; }
    public Guid? ExitUserId { get; set; }
    public StayStatus Status { get; set; }
    public string? CancelReason { get; set; }
}
