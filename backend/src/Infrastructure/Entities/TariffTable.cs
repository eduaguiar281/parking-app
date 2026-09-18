using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Infrastructure.Entities;

public sealed class TariffTable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public Guid? SectorId { get; set; }
    public Sector? Sector { get; set; }
    public decimal FirstHourAmount { get; set; }
    public decimal AdditionalHourAmount { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public TariffStatus Status { get; set; }
}
