using System.Net;
using ParkingApp.Api.Tests.Api;
using Xunit;

namespace ParkingApp.Api.Tests;

public class AliveEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AliveEndpointTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task GetAlive_servicoEmExecucao_retornaEstouVivo()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/alive");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("estou vivo", body);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetApi_semCookie_retorna401()
    {
        var client = _factory.CreateClient(new() { AllowAutoRedirect = false });
        var response = await client.GetAsync("/api/session");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
