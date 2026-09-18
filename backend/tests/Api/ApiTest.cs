using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public abstract class ApiTest : IDisposable
{
    protected ApiFactory Factory { get; } = new();

    public void Dispose()
    {
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }

    protected async Task<HttpClient> AdminClient()
    {
        var client = Factory.CreateClient(new() { HandleCookies = true, AllowAutoRedirect = false });
        var login = await LoginAsync(client, "admin", "admin123");
        login.EnsureSuccessStatusCode();
        return client;
    }

    protected static Task<HttpResponseMessage> LoginAsync(HttpClient client, string login, string password) =>
        client.PostAsJsonAsync("/api/session", new { login, password });

    protected static async Task<Guid> CreateSector(HttpClient client, string code, string name, string category)
    {
        var response = await client.PostAsJsonAsync("/api/sectors", new
        {
            name,
            code,
            allowedCategories = category,
            status = "Active"
        });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }

    protected sealed record Setup(Guid SectorId, Guid SpotId, Guid OtherSpotId);

    protected async Task<(HttpClient Client, Guid StayId, Setup Setup)> ParkedCar(bool openCash = true)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var client = await AdminClient();
        var sectorId = await CreateSector(client, "B" + suffix, "Pátio B " + suffix, "Car");
        var batch = await client.PostAsJsonAsync("/api/spots/batch", new { sectorId, prefix = "B" + suffix + "-", start = 1, end = 2 });
        batch.EnsureSuccessStatusCode();
        var spots = await client.GetFromJsonAsync<JsonElement>($"/api/spots?sectorId={sectorId}");
        var spotId = spots[0].GetProperty("id").GetGuid();
        var other = spots[1].GetProperty("id").GetGuid();
        var tariff = await client.PostAsJsonAsync("/api/tariffs", new
        {
            name = "Carro " + suffix,
            vehicleType = "Car",
            sectorId,
            firstHourAmount = 10,
            additionalHourAmount = 5,
            effectiveFrom = "2020-01-01",
            status = "Active"
        });
        tariff.EnsureSuccessStatusCode();
        if (openCash)
        {
            var cash = await client.PostAsJsonAsync("/api/cash", new
            {
                operationalDate = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                openingAmount = 50
            });
            cash.EnsureSuccessStatusCode();
        }

        var entry = await client.PostAsJsonAsync("/api/stays", new
        {
            plate = "ABC-1234",
            vehicleType = "Car",
            sectorId,
            spotId
        });
        entry.EnsureSuccessStatusCode();
        var stayId = (await entry.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        return (client, stayId, new Setup(sectorId, spotId, other));
    }
}
