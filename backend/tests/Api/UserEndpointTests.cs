using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class UserEndpointTests : ApiTest
{
    [Fact]
    public async Task CreateUser_admin_criaOperador()
    {
        var client = await AdminClient();
        var response = await client.PostAsJsonAsync("/api/users", new
        {
            fullName = "Operador",
            login = "op1",
            password = "senha123",
            role = "Operator",
            status = "Active"
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task DeactivateUser_operador_naoAutentica()
    {
        var client = await AdminClient();
        var created = await client.PostAsJsonAsync("/api/users", new
        {
            fullName = "Op2",
            login = "op2",
            password = "senha123",
            role = "Operator",
            status = "Active"
        });
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        await client.PostAsync($"/api/users/{id}/deactivate", null);
        var anon = Factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await LoginAsync(anon, "op2", "senha123")).StatusCode);
    }
}
