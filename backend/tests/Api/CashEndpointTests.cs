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

    [Fact]
    public async Task CloseCash_outroOperador_gravaClosedBy()
    {
        var admin = await AdminClient();
        var opened = await admin.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-21", openingAmount = 80 });
        var cashId = (await opened.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        var created = await admin.PostAsJsonAsync("/api/users", new
        {
            fullName = "Turno",
            login = "opfecha",
            password = "senha123",
            role = "Operator",
            status = "Active"
        });
        created.EnsureSuccessStatusCode();
        var closerId = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        var openerId = (await (await admin.GetAsync("/api/session")).Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();

        var operatorClient = Factory.CreateClient(new() { HandleCookies = true, AllowAutoRedirect = false });
        (await LoginAsync(operatorClient, "opfecha", "senha123")).EnsureSuccessStatusCode();
        var closed = await operatorClient.PostAsJsonAsync($"/api/cash/{cashId}/close", new { informedAmount = 80, notes = "troca de turno" });
        Assert.Equal(HttpStatusCode.OK, closed.StatusCode);
        var body = await closed.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(closerId, body.GetProperty("closedByUserId").GetGuid());
        Assert.NotEqual(openerId, closerId);
    }
}
