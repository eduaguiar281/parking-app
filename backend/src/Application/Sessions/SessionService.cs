using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Sessions;

public sealed class SessionService(ParkingDbContext db, IPasswordHasher<User> hasher)
{
    public async Task<IResult> CreateAsync(HttpContext http, string login, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Login.ToLower() == login.Trim().ToLower());
        if (user is null || user.Status != UserStatus.Active)
        {
            return HttpError.Json(401, "invalid_credentials", "Login ou senha inválidos.");
        }

        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return HttpError.Json(401, "invalid_credentials", "Login ou senha inválidos.");
        }

        user.LastAccessAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("fullName", user.FullName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Results.Ok(ToCurrentUser(user));
    }

    public async Task<IResult> GetAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId);
        return user is null
            ? HttpError.Json(401, "unauthenticated", "Sessão inválida.")
            : Results.Ok(ToCurrentUser(user));
    }

    public static object ToCurrentUser(User user) => new
    {
        id = user.Id,
        fullName = user.FullName,
        login = user.Login,
        role = user.Role.ToString()
    };
}
