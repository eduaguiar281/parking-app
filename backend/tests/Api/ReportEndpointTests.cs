using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class ReportEndpointTests : ApiTest
{
    [Fact]
    public async Task ReportCash_periodoComFechamento_incluiEsperado()
    {
        var client = await AdminClient();
        var opened = await client.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-21", openingAmount = 10 });
        var id = (await opened.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        await client.PostAsJsonAsync($"/api/cash/{id}/close", new { informedAmount = 10 });
        var report = await client.GetFromJsonAsync<JsonElement>("/api/reports/cash?from=2026-09-21&to=2026-09-21");
        Assert.True(report.GetProperty("items").GetArrayLength() >= 1);
    }

    [Fact]
    public async Task ReportStays_periodo_totaisDeReceita()
    {
        var (client, stayId, _) = await ParkedCar();
        await client.PostAsJsonAsync($"/api/stays/{stayId}/exit", new { paymentMethod = "Pix" });
        var report = await client.GetFromJsonAsync<JsonElement>("/api/reports/stays?from=2020-01-01&to=2030-01-01");
        Assert.True(report.GetProperty("exitCount").GetInt32() >= 1);
    }
}
