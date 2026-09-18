using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Users;

public sealed class UserService(ParkingDbContext db, IPasswordHasher<User> hasher, AuditWriter audit)
{
    public async Task<List<object>> ListAsync()
    {
        var users = await db.Users.OrderBy(u => u.FullName).ToListAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<IResult> CreateAsync(Guid actorId, UserWrite write)
    {
        if (string.IsNullOrWhiteSpace(write.Password))
        {
            return HttpError.Json(400, "password_required", "Senha é obrigatória na criação.");
        }

        if (await db.Users.AnyAsync(u => u.Login.ToLower() == write.Login.Trim().ToLower()))
        {
            return HttpError.Json(409, "duplicate_login", "Já existe um usuário com este login.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = write.FullName,
            Login = write.Login.Trim(),
            Role = write.Role,
            Status = write.Status,
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, write.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        await audit.WriteAsync(actorId, "create", "User", user.Id.ToString(), new { user.Login, user.Role });
        return Results.Created($"/api/users/{user.Id}", ToDto(user));
    }

    public async Task<IResult> UpdateAsync(Guid actorId, Guid id, UserWrite write)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return HttpError.Json(404, "not_found", "Usuário não encontrado.");
        }

        user.FullName = write.FullName;
        user.Login = write.Login.Trim();
        user.Role = write.Role;
        user.Status = write.Status;
        if (!string.IsNullOrWhiteSpace(write.Password))
        {
            user.PasswordHash = hasher.HashPassword(user, write.Password);
        }

        await db.SaveChangesAsync();
        await audit.WriteAsync(actorId, "update", "User", user.Id.ToString(), new { user.Login, user.Status });
        return Results.Ok(ToDto(user));
    }

    public async Task<IResult> SetStatusAsync(Guid actorId, Guid id, UserStatus status)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return HttpError.Json(404, "not_found", "Usuário não encontrado.");
        }

        user.Status = status;
        await db.SaveChangesAsync();
        await audit.WriteAsync(actorId, status == UserStatus.Inactive ? "deactivate" : "activate", "User", user.Id.ToString(), user);
        return Results.NoContent();
    }

    private static object ToDto(User user) => new
    {
        id = user.Id,
        fullName = user.FullName,
        login = user.Login,
        role = user.Role.ToString(),
        status = user.Status.ToString(),
        createdAt = user.CreatedAt,
        lastAccessAt = user.LastAccessAt
    };
}

public sealed record UserWrite(string FullName, string Login, string? Password, UserRole Role, UserStatus Status);
