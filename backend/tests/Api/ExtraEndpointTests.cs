using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class ExtraEndpointTests : ApiTest
{
    [Fact]
    public async Task GetSession_autenticado_retornaPerfil()
    {
        var client = await AdminClient();
        var session = await client.GetFromJsonAsync<JsonElement>("/api/session");
        Assert.Equal("admin", session.GetProperty("login").GetString());
    }

    [Fact]
    public async Task GetSpotsSuggest_comVagaLivre_retornaCodigo()
    {
        var (client, _, setup) = await ParkedCar();
        var response = await client.GetAsync($"/api/spots/suggest?vehicleType=Car&sectorId={setup.SectorId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReopenCash_admin_reabreFechado()
    {
        var client = await AdminClient();
        var opened = await client.PostAsJsonAsync("/api/cash", new { operationalDate = "2026-09-22", openingAmount = 10 });
        var id = (await opened.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        await client.PostAsJsonAsync($"/api/cash/{id}/close", new { informedAmount = 10 });
        var reopen = await client.PostAsJsonAsync($"/api/cash/{id}/reopen", new { reason = "conferencia" });
        Assert.Equal(HttpStatusCode.OK, reopen.StatusCode);
    }

    [Fact]
    public async Task ActivateUser_inativo_voltaAAutenticar()
    {
        var client = await AdminClient();
        var created = await client.PostAsJsonAsync("/api/users", new
        {
            fullName = "Op3",
            login = "op3",
            password = "senha123",
            role = "Operator",
            status = "Inactive"
        });
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        await client.PostAsync($"/api/users/{id}/activate", null);
        var anon = Factory.CreateClient();
        Assert.Equal(HttpStatusCode.OK, (await LoginAsync(anon, "op3", "senha123")).StatusCode);
    }

    [Fact]
    public async Task GetStayPreview_ativa_retornaValorProposto()
    {
        var (client, stayId, _) = await ParkedCar();
        var preview = await client.GetFromJsonAsync<JsonElement>($"/api/stays/{stayId}/exit-preview");
        Assert.True(preview.GetProperty("proposedAmount").GetDecimal() > 0);
    }

    [Fact]
    public async Task ExportCsv_admin_retornaTextCsv()
    {
        var client = await AdminClient();
        var response = await client.GetAsync("/api/reports/stays.csv?from=2026-01-01&to=2026-12-31");
        Assert.StartsWith("text/", response.Content.Headers.ContentType?.MediaType);
    }
}
