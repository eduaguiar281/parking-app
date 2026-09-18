using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Tariffs;

public sealed class TariffService(ParkingDbContext db, AuditWriter audit)
{
    public Task<List<TariffTable>> ListAsync() => db.Tariffs.OrderBy(t => t.Name).ToListAsync();

    public async Task<IResult> CreateAsync(Guid userId, TariffWrite write)
    {
        if (Overlaps(write, excludingId: null))
        {
            return HttpError.Json(409, "tariff_conflict", "Já existe tarifa ativa com a mesma abrangência e vigência.");
        }

        var tariff = Map(write, Guid.NewGuid());
        db.Tariffs.Add(tariff);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "create", "TariffTable", tariff.Id.ToString(), tariff);
        return Results.Created($"/api/tariffs/{tariff.Id}", tariff);
    }

    public async Task<IResult> UpdateAsync(Guid userId, Guid id, TariffWrite write)
    {
        var tariff = await db.Tariffs.FindAsync(id);
        if (tariff is null)
        {
            return HttpError.Json(404, "not_found", "Tarifa não encontrada.");
        }

        if (Overlaps(write, excludingId: id))
        {
            return HttpError.Json(409, "tariff_conflict", "Já existe tarifa ativa com a mesma abrangência e vigência.");
        }

        tariff.Name = write.Name;
        tariff.VehicleType = write.VehicleType;
        tariff.SectorId = write.SectorId;
        tariff.FirstHourAmount = write.FirstHourAmount;
        tariff.AdditionalHourAmount = write.AdditionalHourAmount;
        tariff.EffectiveFrom = write.EffectiveFrom;
        tariff.EffectiveTo = write.EffectiveTo;
        tariff.Status = write.Status;
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "update", "TariffTable", tariff.Id.ToString(), tariff);
        return Results.Ok(tariff);
    }

    public async Task<IResult> DeactivateAsync(Guid userId, Guid id)
    {
        var tariff = await db.Tariffs.FindAsync(id);
        if (tariff is null)
        {
            return HttpError.Json(404, "not_found", "Tarifa não encontrada.");
        }

        tariff.Status = TariffStatus.Inactive;
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "deactivate", "TariffTable", tariff.Id.ToString(), tariff);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        if (await db.Stays.AnyAsync(s => s.TariffTableId == id))
        {
            return HttpError.Json(409, "tariff_in_use", "Tarifa já utilizada não pode ser excluída; inative-a.");
        }

        var tariff = await db.Tariffs.FindAsync(id);
        if (tariff is null)
        {
            return HttpError.Json(404, "not_found", "Tarifa não encontrada.");
        }

        db.Tariffs.Remove(tariff);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private bool Overlaps(TariffWrite write, Guid? excludingId)
    {
        if (write.Status != TariffStatus.Active)
        {
            return false;
        }

        var from = write.EffectiveFrom;
        var to = write.EffectiveTo ?? DateOnly.MaxValue;
        return db.Tariffs.AsEnumerable().Any(t =>
            t.Status == TariffStatus.Active &&
            t.VehicleType == write.VehicleType &&
            t.SectorId == write.SectorId &&
            (excludingId is null || t.Id != excludingId) &&
            RangesOverlap(t.EffectiveFrom, t.EffectiveTo ?? DateOnly.MaxValue, from, to));
    }

    private static bool RangesOverlap(DateOnly a1, DateOnly a2, DateOnly b1, DateOnly b2) => a1 <= b2 && b1 <= a2;

    private static TariffTable Map(TariffWrite write, Guid id) => new()
    {
        Id = id,
        Name = write.Name,
        VehicleType = write.VehicleType,
        SectorId = write.SectorId,
        FirstHourAmount = write.FirstHourAmount,
        AdditionalHourAmount = write.AdditionalHourAmount,
        EffectiveFrom = write.EffectiveFrom,
        EffectiveTo = write.EffectiveTo,
        Status = write.Status
    };
}

public sealed record TariffWrite(
    string Name,
    VehicleType VehicleType,
    Guid? SectorId,
    decimal FirstHourAmount,
    decimal AdditionalHourAmount,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    TariffStatus Status);
