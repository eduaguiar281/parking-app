using ParkingApp.Api.Application.Sectors;
using ParkingApp.Api.Application.Spots;
using ParkingApp.Api.Application.Tariffs;
using ParkingApp.Api.Application.Users;
using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Api;

public static class CatalogEndpoints
{
    public static RouteGroupBuilder MapCatalog(this RouteGroupBuilder api)
    {
        api.MapGet("/users", async (HttpContext http, UserService users) =>
            http.User.AdminOnly() ?? Results.Ok(await users.ListAsync()));
        api.MapPost("/users", async (HttpContext http, UserService users, UserWrite body) =>
            http.User.AdminOnly() ?? await users.CreateAsync(http.User.UserId(), body));
        api.MapPut("/users/{id:guid}", async (HttpContext http, UserService users, Guid id, UserWrite body) =>
            http.User.AdminOnly() ?? await users.UpdateAsync(http.User.UserId(), id, body));
        api.MapPost("/users/{id:guid}/deactivate", async (HttpContext http, UserService users, Guid id) =>
            http.User.AdminOnly() ?? await users.SetStatusAsync(http.User.UserId(), id, UserStatus.Inactive));
        api.MapPost("/users/{id:guid}/activate", async (HttpContext http, UserService users, Guid id) =>
            http.User.AdminOnly() ?? await users.SetStatusAsync(http.User.UserId(), id, UserStatus.Active));

        api.MapGet("/sectors", async (SectorService sectors) =>
            Results.Ok(await sectors.ListAsync()));
        api.MapPost("/sectors", async (HttpContext http, SectorService sectors, SectorWrite body) =>
            http.User.AdminOnly() ?? await sectors.CreateAsync(http.User.UserId(), body));
        api.MapPut("/sectors/{id:guid}", async (HttpContext http, SectorService sectors, Guid id, SectorWrite body) =>
            http.User.AdminOnly() ?? await sectors.UpdateAsync(http.User.UserId(), id, body));

        api.MapGet("/spots", async (SpotService spots, Guid? sectorId, VehicleType? vehicleType, bool onlyAvailable = false) =>
            Results.Ok(await spots.ListAsync(sectorId, vehicleType, onlyAvailable)));
        api.MapPost("/spots", async (HttpContext http, SpotService spots, SpotWrite body) =>
            http.User.AdminOnly() ?? await spots.CreateAsync(http.User.UserId(), body));
        api.MapPost("/spots/batch", async (HttpContext http, SpotService spots, BatchWrite body) =>
            http.User.AdminOnly() ?? await spots.CreateBatchAsync(http.User.UserId(), body.SectorId, body.Prefix, body.Start, body.End));
        api.MapGet("/spots/suggest", async (SpotService spots, VehicleType vehicleType, Guid? sectorId) =>
            await spots.SuggestAsync(vehicleType, sectorId));
        api.MapPut("/spots/{id:guid}", async (HttpContext http, SpotService spots, Guid id, SpotWrite body) =>
            http.User.AdminOnly() ?? await spots.UpdateAsync(http.User.UserId(), id, body));

        api.MapGet("/tariffs", async (HttpContext http, TariffService tariffs) =>
            http.User.AdminOnly() ?? Results.Ok(await tariffs.ListAsync()));
        api.MapPost("/tariffs", async (HttpContext http, TariffService tariffs, TariffWrite body) =>
            http.User.AdminOnly() ?? await tariffs.CreateAsync(http.User.UserId(), body));
        api.MapPut("/tariffs/{id:guid}", async (HttpContext http, TariffService tariffs, Guid id, TariffWrite body) =>
            http.User.AdminOnly() ?? await tariffs.UpdateAsync(http.User.UserId(), id, body));
        api.MapPost("/tariffs/{id:guid}/deactivate", async (HttpContext http, TariffService tariffs, Guid id) =>
            http.User.AdminOnly() ?? await tariffs.DeactivateAsync(http.User.UserId(), id));
        api.MapDelete("/tariffs/{id:guid}", async (HttpContext http, TariffService tariffs, Guid id) =>
            http.User.AdminOnly() ?? await tariffs.DeleteAsync(id));

        return api;
    }
}

public sealed record BatchWrite(Guid SectorId, string Prefix, int Start, int End);
