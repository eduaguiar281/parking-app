using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class TariffSelectorTests
{
    [Fact]
    public void SelectTariff_geralESetor_escolheEspecifica()
    {
        var sectorId = Guid.NewGuid();
        var general = new TariffOption(Guid.NewGuid(), VehicleType.Car, null, new DateOnly(2026, 1, 1), null, TariffStatus.Active);
        var specific = new TariffOption(Guid.NewGuid(), VehicleType.Car, sectorId, new DateOnly(2026, 1, 1), null, TariffStatus.Active);
        var chosen = TariffSelector.Select([general, specific], VehicleType.Car, sectorId, new DateOnly(2026, 9, 18));
        Assert.Equal(specific.Id, chosen?.Id);
    }
}
