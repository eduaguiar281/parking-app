using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class SpotEndpointTests : ApiTest
{
    [Fact]
    public async Task CreateSpotBatch_intervaloLivre_criaTodas()
    {
        var client = await AdminClient();
        var sectorId = await CreateSector(client, "PA", "Pátio A", "Motorcycle");
        var response = await client.PostAsJsonAsync("/api/spots/batch", new { sectorId, prefix = "P-A-", start = 1, end = 3 });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateSpotBatch_codigoExistente_naoCriaNada()
    {
        var client = await AdminClient();
        var sectorId = await CreateSector(client, "PX", "Pátio X", "Car");
        await client.PostAsJsonAsync("/api/spots/batch", new { sectorId, prefix = "P-X-", start = 1, end = 2 });
        var second = await client.PostAsJsonAsync("/api/spots/batch", new { sectorId, prefix = "P-X-", start = 1, end = 2 });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
