using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class SessionEndpointTests : ApiTest
{
    [Fact]
    public async Task CreateSession_credenciaisValidas_defineCookie()
    {
        var client = Factory.CreateClient(new() { HandleCookies = false, AllowAutoRedirect = false });
        var response = await LoginAsync(client, "admin", "admin123");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(response.Headers, h => h.Key == "Set-Cookie");
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), v => v.Contains("ParkingApp.Session"));
    }

    [Fact]
    public async Task CreateSession_senhaInvalida_retorna401()
    {
        var client = Factory.CreateClient();
        var response = await LoginAsync(client, "admin", "errada");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSession_usuarioInativo_retorna401()
    {
        var client = await AdminClient();
        var created = await client.PostAsJsonAsync("/api/users", new
        {
            fullName = "Op",
            login = "inativo1",
            password = "senha123",
            role = "Operator",
            status = "Inactive"
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var anon = Factory.CreateClient();
        var response = await LoginAsync(anon, "inativo1", "senha123");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
