using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class CashEndpointTests : ApiTest
{
    [Fact]
    public async Task OpenCash_jaExisteAberto_retorna409()
    {
        var client = await AdminClient();
        await client.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-18", openingAmount = 100 });
        var second = await client.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-19", openingAmount = 50 });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task CloseCash_comMovimentos_gravaEsperadoEDiferenca()
    {
        var client = await AdminClient();
        var opened = await client.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-20", openingAmount = 100 });
        var cash = await opened.Content.ReadFromJsonAsync<JsonElement>();
        var id = cash.GetProperty("id").GetGuid();
        await client.PostAsJsonAsync($"/api/cash/{id}/movements", new { type = "Supply", amount = 20, reason = "troco" });
        var closed = await client.PostAsJsonAsync($"/api/cash/{id}/close", new { informedAmount = 110, notes = "ok" });
        var raw = await closed.Content.ReadAsStringAsync();
        Assert.True(closed.IsSuccessStatusCode, raw);
        var body = JsonSerializer.Deserialize<JsonElement>(raw);
        Assert.Equal(HttpStatusCode.OK, closed.StatusCode);
        Assert.Equal(120m, body.GetProperty("expectedAmount").GetDecimal());
        Assert.Equal(-10m, body.GetProperty("difference").GetDecimal());
    }
}
