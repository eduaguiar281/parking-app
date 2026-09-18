using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class StayEntryEndpointTests : ApiTest
{
    [Fact]
    public async Task RegisterEntry_vagaLivre_ocupa()
    {
        var (_, stayId, _) = await ParkedCar();
        Assert.NotEqual(Guid.Empty, stayId);
    }

    [Fact]
    public async Task RegisterEntry_placaAtiva_retorna409()
    {
        var (client, _, setup) = await ParkedCar();
        var again = await client.PostAsJsonAsync("/api/stays", new
        {
            plate = "ABC-1234",
            vehicleType = "Car",
            sectorId = setup.SectorId,
            spotId = setup.OtherSpotId
        });
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task RegisterEntry_vagaDisputada_segundaRetorna409()
    {
        var (client, _, setup) = await ParkedCar();
        var second = await client.PostAsJsonAsync("/api/stays", new
        {
            plate = "DEF-5678",
            vehicleType = "Car",
            sectorId = setup.SectorId,
            spotId = setup.SpotId
        });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
