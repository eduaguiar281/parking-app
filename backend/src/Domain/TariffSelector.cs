namespace ParkingApp.Api.Domain;

public sealed record TariffOption(
    Guid Id,
    VehicleType VehicleType,
    Guid? SectorId,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    TariffStatus Status);

public static class TariffSelector
{
    public static TariffOption? Select(
        IEnumerable<TariffOption> tariffs,
        VehicleType vehicleType,
        Guid sectorId,
        DateOnly onDate)
    {
        var eligible = tariffs
            .Where(t => t.Status == TariffStatus.Active)
            .Where(t => t.VehicleType == vehicleType)
            .Where(t => t.EffectiveFrom <= onDate && (t.EffectiveTo is null || t.EffectiveTo >= onDate))
            .ToList();

        return eligible.FirstOrDefault(t => t.SectorId == sectorId)
            ?? eligible.FirstOrDefault(t => t.SectorId is null);
    }
}
