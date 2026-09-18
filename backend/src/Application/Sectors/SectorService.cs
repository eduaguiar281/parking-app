using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Sectors;

public sealed class SectorService(ParkingDbContext db, AuditWriter audit)
{
    public Task<List<Sector>> ListAsync() => db.Sectors.OrderBy(s => s.Code).ToListAsync();

    public async Task<IResult> CreateAsync(Guid userId, SectorWrite write)
    {
        if (await db.Sectors.AnyAsync(s => s.Code == write.Code))
        {
            return HttpError.Json(409, "duplicate_code", "Já existe um setor com este código.");
        }

        var sector = new Sector
        {
            Id = Guid.NewGuid(),
            Name = write.Name,
            Code = write.Code.Trim(),
            Description = write.Description,
            AllowedCategories = write.AllowedCategories,
            Status = write.Status
        };
        db.Sectors.Add(sector);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "create", "Sector", sector.Id.ToString(), sector);
        return Results.Created($"/api/sectors/{sector.Id}", sector);
    }

    public async Task<IResult> UpdateAsync(Guid userId, Guid id, SectorWrite write)
    {
        var sector = await db.Sectors.FindAsync(id);
        if (sector is null)
        {
            return HttpError.Json(404, "not_found", "Setor não encontrado.");
        }

        sector.Name = write.Name;
        sector.Code = write.Code.Trim();
        sector.Description = write.Description;
        sector.AllowedCategories = write.AllowedCategories;
        sector.Status = write.Status;
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "update", "Sector", sector.Id.ToString(), sector);
        return Results.Ok(sector);
    }
}

public sealed record SectorWrite(
    string Name,
    string Code,
    string? Description,
    VehicleCategory AllowedCategories,
    SectorStatus Status);
