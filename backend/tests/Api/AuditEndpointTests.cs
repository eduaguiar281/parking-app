using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class AuditEndpointTests : ApiTest
{
    [Fact]
    public async Task ListAudit_aposEntrada_contemUsuarioEAcao()
    {
        await ParkedCar();
        var client = await AdminClient();
        var events = await client.GetFromJsonAsync<JsonElement>("/api/audit");
        Assert.True(events.GetArrayLength() >= 1);
        Assert.False(string.IsNullOrWhiteSpace(events[0].GetProperty("action").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(events[0].GetProperty("userName").GetString()));
    }
}
