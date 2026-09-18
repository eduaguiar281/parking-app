using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Infrastructure.Entities;

public sealed class Spot
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
    public VehicleType VehicleCategory { get; set; }
    public SpotStatus Status { get; set; }
}
