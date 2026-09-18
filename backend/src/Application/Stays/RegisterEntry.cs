using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Application.Spots;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Stays;

public sealed class RegisterEntry(ParkingDbContext db, AuditWriter audit)
{
    public async Task<IResult> ExecuteAsync(Guid userId, EntryWrite write)
    {
        if (!Plate.IsValid(write.Plate))
        {
            return HttpError.Json(400, "invalid_plate", "Placa inválida. Use ABC-1234 ou ABC1D23.");
        }

        var plate = Plate.Normalize(write.Plate);
        if (await db.Stays.AnyAsync(s => s.Plate == plate && s.Status == StayStatus.Active))
        {
            return HttpError.Json(409, "plate_active", "Já existe estadia ativa para esta placa.");
        }

        var sector = await db.Sectors.FindAsync(write.SectorId);
        var spot = await db.Spots.FindAsync(write.SpotId);
        if (sector is null || spot is null || spot.SectorId != sector.Id)
        {
            return HttpError.Json(400, "invalid_spot", "Setor ou vaga inválidos.");
        }

        if (sector.Status != SectorStatus.Active)
        {
            return HttpError.Json(400, "inactive_sector", "Setor inativo não recebe entrada.");
        }

        if (!SpotService.IsCompatible(sector.AllowedCategories, write.VehicleType) ||
            spot.VehicleCategory != write.VehicleType)
        {
            return HttpError.Json(400, "incompatible", "Tipo de veículo incompatível com setor ou vaga.");
        }

        if (spot.Status is SpotStatus.Blocked or SpotStatus.Maintenance)
        {
            return HttpError.Json(400, "spot_unavailable", "Vaga bloqueada ou em manutenção.");
        }

        var today = BrazilianTime.Today();
        var tariffs = await db.Tariffs.ToListAsync();
        var option = TariffSelector.Select(
            tariffs.Select(t => new TariffOption(t.Id, t.VehicleType, t.SectorId, t.EffectiveFrom, t.EffectiveTo, t.Status)),
            write.VehicleType,
            sector.Id,
            today);
        if (option is null)
        {
            return HttpError.Json(400, "no_tariff", "Não há tarifa ativa compatível para este tipo e setor.");
        }

        var tariff = await db.Tariffs.FindAsync(option.Id);
        if (tariff is null)
        {
            return HttpError.Json(400, "no_tariff", "Não há tarifa ativa compatível para este tipo e setor.");
        }

        var updated = await db.Spots
            .Where(s => s.Id == spot.Id && s.Status == SpotStatus.Free)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, SpotStatus.Occupied));
        if (updated == 0)
        {
            return HttpError.Json(409, "spot_taken", "A vaga já não está livre.");
        }

        var stay = new ParkingStay
        {
            Id = Guid.NewGuid(),
            Plate = plate,
            VehicleType = write.VehicleType,
            SectorId = sector.Id,
            SectorCodeSnapshot = sector.Code,
            SectorNameSnapshot = sector.Name,
            SpotId = spot.Id,
            SpotCodeSnapshot = spot.Code,
            TariffTableId = tariff.Id,
            TariffNameSnapshot = tariff.Name,
            FirstHourAmountSnapshot = tariff.FirstHourAmount,
            AdditionalHourAmountSnapshot = tariff.AdditionalHourAmount,
            EntryAt = DateTimeOffset.UtcNow,
            EntryUserId = userId,
            Status = StayStatus.Active
        };
        db.Stays.Add(stay);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            await db.Spots.Where(s => s.Id == spot.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, SpotStatus.Free));
            return HttpError.Json(409, "spot_taken", "A vaga já não está livre.");
        }

        await audit.WriteAsync(userId, "entry", "ParkingStay", stay.Id.ToString(), stay);
        return Results.Created($"/api/stays/{stay.Id}", StayMapper.ToDto(stay));
    }
}

public sealed record EntryWrite(string Plate, VehicleType VehicleType, Guid SectorId, Guid SpotId);
