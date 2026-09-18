using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class StayExitEndpointTests : ApiTest
{
    [Fact]
    public async Task ConfirmExit_semCaixaAberto_retorna409()
    {
        var (client, stayId, _) = await ParkedCar(openCash: false);
        var response = await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Pix" });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmExit_pago_imutaValor()
    {
        var (client, stayId, _) = await ParkedCar();
        var exit = await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Pix" });
        Assert.Equal(HttpStatusCode.OK, exit.StatusCode);
        var first = await exit.Content.ReadFromJsonAsync<JsonElement>();
        var amount = first.GetProperty("amountCharged").GetDecimal();
        var tariffId = (await client.GetFromJsonAsync<JsonElement>("/api/tariffs"))[0].GetProperty("id").GetGuid();
        await client.PutAsJsonAsync($"/api/tariffs/{tariffId}", new
        {
            name = "Carro geral",
            vehicleType = "Car",
            firstHourAmount = 99,
            additionalHourAmount = 99,
            effectiveFrom = "2026-01-01",
            status = "Active"
        });
        var receipt = await client.GetFromJsonAsync<JsonElement>($"/api/stays/{stayId}/receipt");
        Assert.Equal(amount, receipt.GetProperty("amountCharged").GetDecimal());
    }

    [Fact]
    public async Task CancelStay_ativa_liberaVaga()
    {
        var (client, stayId, _) = await ParkedCar();
        var response = await client.PostAsJsonAsync($"/api/stays/{stayId}/cancel", new { reason = "erro de placa" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
