using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class DashboardEndpointTests : ApiTest
{
    [Fact]
    public async Task GetDashboard_aposEntradaESaida_totaisConferem()
    {
        var (client, stayId, _) = await ParkedCar();
        await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Cash" });
        var response = await client.GetAsync("/api/operations/dashboard");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var dash = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(0, dash.GetProperty("occupied").GetInt32());
        Assert.True(dash.GetProperty("free").GetInt32() >= 1);
    }
}
