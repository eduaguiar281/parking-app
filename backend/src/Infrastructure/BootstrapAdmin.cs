using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Infrastructure;

public static class BootstrapAdmin
{
    public static async Task EnsureAsync(ParkingDbContext db, IPasswordHasher<User> hasher, string password)
    {
        await EnsureUserAsync(db, hasher, "admin", "Administrador", UserRole.Administrator, password);
        await EnsureUserAsync(db, hasher, "operador", "Operador", UserRole.Operator, password);
    }

    private static async Task EnsureUserAsync(
        ParkingDbContext db,
        IPasswordHasher<User> hasher,
        string login,
        string fullName,
        UserRole role,
        string password)
    {
        if (await db.Users.AnyAsync(u => u.Login.ToLower() == login))
        {
            return;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Login = login,
            Role = role,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
