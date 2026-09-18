using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Infrastructure;

public static class BootstrapAdmin
{
    public static async Task EnsureAsync(ParkingDbContext db, IPasswordHasher<User> hasher, string password)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Administrador",
            Login = "admin",
            Role = UserRole.Administrator,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, password);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
