using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class TariffEndpointTests : ApiTest
{
    [Fact]
    public async Task CreateTariff_abrangenciaSobreposta_retorna409()
    {
        var client = await AdminClient();
        var body = new
        {
            name = "Carro geral",
            vehicleType = "Car",
            firstHourAmount = 10,
            additionalHourAmount = 5,
            effectiveFrom = "2026-01-01",
            status = "Active"
        };
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/tariffs", body)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/tariffs", body)).StatusCode);
    }

    [Fact]
    public async Task DeleteTariff_jaUtilizada_naoPermitido()
    {
        var (client, _, _) = await ParkedCar();
        var tariffs = await client.GetFromJsonAsync<JsonElement>("/api/tariffs");
        var id = tariffs[0].GetProperty("id").GetGuid();
        var response = await client.DeleteAsync($"/api/tariffs/{id}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
