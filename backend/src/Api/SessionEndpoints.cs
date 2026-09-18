using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ParkingApp.Api.Application.Sessions;

namespace ParkingApp.Api.Api;

public static class SessionEndpoints
{
    public static RouteGroupBuilder MapSession(this RouteGroupBuilder api)
    {
        api.MapPost("/session", async (HttpContext http, SessionService sessions, SessionWrite body) =>
            await sessions.CreateAsync(http, body.Login, body.Password)).AllowAnonymous();

        api.MapGet("/session", async (HttpContext http, SessionService sessions) =>
            await sessions.GetAsync(http.User.UserId()));

        api.MapDelete("/session", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.NoContent();
        });

        return api;
    }
}

public sealed record SessionWrite(string Login, string Password);
