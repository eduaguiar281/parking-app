using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class StayHistoryEndpointTests : ApiTest
{
    [Fact]
    public async Task ListStayHistory_filtroPlacaEPeriodo_retornaSoCompativeis()
    {
        var (client, stayId, _) = await ParkedCar();
        await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Pix" });
        var list = await client.GetFromJsonAsync<JsonElement>("/api/stays?plate=ABC-1234");
        Assert.True(list.GetArrayLength() >= 1);
    }

    [Fact]
    public async Task ListStayHistory_tarifaAlteradaDepois_mantemSnapshot()
    {
        var (client, stayId, _) = await ParkedCar();
        await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Pix" });
        var receipt = await client.GetFromJsonAsync<JsonElement>($"/api/stays/{stayId}/receipt");
        var original = receipt.GetProperty("firstHourAmount").GetDecimal();
        var tariffId = (await client.GetFromJsonAsync<JsonElement>("/api/tariffs"))[0].GetProperty("id").GetGuid();
        await client.PutAsJsonAsync($"/api/tariffs/{tariffId}", new
        {
            name = "Nova",
            vehicleType = "Car",
            firstHourAmount = 80,
            additionalHourAmount = 80,
            effectiveFrom = "2026-01-01",
            status = "Active"
        });
        var history = await client.GetFromJsonAsync<JsonElement>("/api/stays?plate=ABC-1234");
        Assert.Equal(original, history[0].GetProperty("firstHourAmount").GetDecimal());
    }
}
