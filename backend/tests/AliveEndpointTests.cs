using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ParkingApp.Api.Tests;

public class AliveEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AliveEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

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
}
