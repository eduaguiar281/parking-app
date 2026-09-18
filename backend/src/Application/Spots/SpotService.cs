using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Spots;

public sealed class SpotService(ParkingDbContext db, AuditWriter audit)
{
    public async Task<List<Spot>> ListAsync(Guid? sectorId, VehicleType? vehicleType, bool onlyAvailable)
    {
        var query = db.Spots.Include(s => s.Sector).AsQueryable();
        if (sectorId is not null)
        {
            query = query.Where(s => s.SectorId == sectorId);
        }

        if (vehicleType is not null)
        {
            query = query.Where(s => s.VehicleCategory == vehicleType);
        }

        if (onlyAvailable)
        {
            query = query.Where(s =>
                s.Status == SpotStatus.Free &&
                s.Sector.Status == SectorStatus.Active);
        }

        return await query.OrderBy(s => s.Code).ToListAsync();
    }

    public async Task<IResult> CreateAsync(Guid userId, SpotWrite write)
    {
        var sector = await db.Sectors.FindAsync(write.SectorId);
        if (sector is null)
        {
            return HttpError.Json(400, "invalid_sector", "Setor não encontrado.");
        }

        if (!IsCompatible(sector.AllowedCategories, write.VehicleCategory))
        {
            return HttpError.Json(400, "incompatible_category", "A categoria da vaga não é compatível com o setor.");
        }

        if (await db.Spots.AnyAsync(s => s.Code == write.Code))
        {
            return HttpError.Json(409, "duplicate_code", "Já existe uma vaga com este código.");
        }

        var spot = new Spot
        {
            Id = Guid.NewGuid(),
            Code = write.Code.Trim(),
            SectorId = write.SectorId,
            VehicleCategory = write.VehicleCategory,
            Status = write.Status == SpotStatus.Occupied ? SpotStatus.Free : write.Status
        };
        db.Spots.Add(spot);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "create", "Spot", spot.Id.ToString(), spot);
        return Results.Created($"/api/spots/{spot.Id}", spot);
    }

    public async Task<IResult> CreateBatchAsync(Guid userId, Guid sectorId, string prefix, int start, int end)
    {
        if (end < start)
        {
            return HttpError.Json(400, "invalid_range", "O intervalo de vagas é inválido.");
        }

        var sector = await db.Sectors.FindAsync(sectorId);
        if (sector is null)
        {
            return HttpError.Json(400, "invalid_sector", "Setor não encontrado.");
        }

        var width = Math.Max(end.ToString().Length, 3);
        var codes = Enumerable.Range(start, end - start + 1)
            .Select(n => $"{prefix}{n.ToString().PadLeft(width, '0')}")
            .ToList();

        if (await db.Spots.AnyAsync(s => codes.Contains(s.Code)))
        {
            return HttpError.Json(409, "duplicate_code", "Algum código do intervalo já existe; nada foi criado.");
        }

        var category = sector.AllowedCategories == VehicleCategory.Motorcycle
            ? VehicleType.Motorcycle
            : VehicleType.Car;

        var spots = codes.Select(code => new Spot
        {
            Id = Guid.NewGuid(),
            Code = code,
            SectorId = sectorId,
            VehicleCategory = category,
            Status = SpotStatus.Free
        }).ToList();

        db.Spots.AddRange(spots);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "create_batch", "Spot", sectorId.ToString(), new { prefix, start, end, count = spots.Count });
        return Results.Created("/api/spots/batch", spots);
    }

    public async Task<IResult> UpdateAsync(Guid userId, Guid id, SpotWrite write)
    {
        var spot = await db.Spots.FindAsync(id);
        if (spot is null)
        {
            return HttpError.Json(404, "not_found", "Vaga não encontrada.");
        }

        if (spot.Status == SpotStatus.Occupied &&
            (write.Status == SpotStatus.Blocked || write.Status == SpotStatus.Maintenance))
        {
            return HttpError.Json(409, "spot_occupied", "Vaga ocupada não vai para bloqueada ou manutenção.");
        }

        spot.Code = write.Code.Trim();
        spot.VehicleCategory = write.VehicleCategory;
        if (write.Status != SpotStatus.Occupied)
        {
            spot.Status = write.Status;
        }

        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "update", "Spot", spot.Id.ToString(), spot);
        return Results.Ok(spot);
    }

    public async Task<IResult> SuggestAsync(VehicleType vehicleType, Guid? sectorId)
    {
        var query = db.Spots.Include(s => s.Sector)
            .Where(s => s.Status == SpotStatus.Free)
            .Where(s => s.Sector.Status == SectorStatus.Active)
            .Where(s => s.VehicleCategory == vehicleType);

        if (sectorId is not null)
        {
            query = query.Where(s => s.SectorId == sectorId);
        }

        var spot = await query.OrderBy(s => s.Code).FirstOrDefaultAsync();
        return spot is null
            ? HttpError.Json(404, "no_spot", "Não há vaga compatível livre.")
            : Results.Ok(spot);
    }

    public static bool IsCompatible(VehicleCategory sector, VehicleType vehicle) =>
        sector == VehicleCategory.Both
        || (sector == VehicleCategory.Car && vehicle == VehicleType.Car)
        || (sector == VehicleCategory.Motorcycle && vehicle == VehicleType.Motorcycle);
}

public sealed record SpotWrite(
    string Code,
    Guid SectorId,
    VehicleType VehicleCategory,
    SpotStatus Status);
