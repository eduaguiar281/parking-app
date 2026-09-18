using System.Security.Claims;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;

namespace ParkingApp.Api.Api;

public static class RequireRole
{
    public static Guid UserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static bool IsAdmin(this ClaimsPrincipal user) =>
        user.IsInRole(nameof(UserRole.Administrator));

    public static IResult? AdminOnly(this ClaimsPrincipal user) =>
        user.IsAdmin() ? null : HttpError.Json(403, "forbidden", "Acesso restrito ao administrador.");
}
