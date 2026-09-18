using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Application.Cash;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Stays;

public sealed class ExitStay(ParkingDbContext db, AuditWriter audit)
{
    public async Task<IResult> PreviewAsync(Guid id)
    {
        var stay = await db.Stays.FindAsync(id);
        if (stay is null || stay.Status != StayStatus.Active)
        {
            return HttpError.Json(409, "not_active", "Não há estadia ativa.");
        }

        if (!await db.CashRegisters.AnyAsync(c => c.Status == CashStatus.Open))
        {
            return HttpError.Json(409, "no_open_cash", "Saída paga exige caixa aberto.");
        }

        var now = DateTimeOffset.UtcNow;
        var duration = now - stay.EntryAt;
        var amount = StayPricing.Calculate(duration, stay.FirstHourAmountSnapshot, stay.AdditionalHourAmountSnapshot);
        return Results.Ok(new
        {
            id = stay.Id,
            plate = stay.Plate,
            vehicleType = stay.VehicleType.ToString(),
            sectorCode = stay.SectorCodeSnapshot,
            sectorName = stay.SectorNameSnapshot,
            spotCode = stay.SpotCodeSnapshot,
            tariffName = stay.TariffNameSnapshot,
            firstHourAmount = stay.FirstHourAmountSnapshot,
            additionalHourAmount = stay.AdditionalHourAmountSnapshot,
            entryAt = stay.EntryAt,
            proposedExitAt = now,
            proposedDurationMinutes = (int)Math.Ceiling(duration.TotalMinutes),
            proposedAmount = amount,
            status = stay.Status.ToString(),
            entryUserName = stay.EntryUserId.ToString()
        });
    }

    public async Task<IResult> ConfirmAsync(Guid userId, Guid id, PaymentMethod paymentMethod)
    {
        var stay = await db.Stays.FindAsync(id);
        if (stay is null || stay.Status != StayStatus.Active)
        {
            return HttpError.Json(409, "not_active", "Não há estadia ativa.");
        }

        var cash = await db.CashRegisters.Include(c => c.Movements)
            .FirstOrDefaultAsync(c => c.Status == CashStatus.Open);
        if (cash is null)
        {
            return HttpError.Json(409, "no_open_cash", "Saída paga exige caixa aberto.");
        }

        var now = DateTimeOffset.UtcNow;
        var duration = now - stay.EntryAt;
        var amount = StayPricing.Calculate(duration, stay.FirstHourAmountSnapshot, stay.AdditionalHourAmountSnapshot);
        stay.ExitAt = now;
        stay.Duration = duration;
        stay.AmountCharged = amount;
        stay.PaymentMethod = paymentMethod;
        stay.CashRegisterId = cash.Id;
        stay.ExitUserId = userId;
        stay.Status = StayStatus.Completed;

        db.CashMovements.Add(new CashMovement
        {
            Id = Guid.NewGuid(),
            CashRegisterId = cash.Id,
            Type = CashMovementType.ExitPayment,
            Amount = amount,
            PaymentMethod = paymentMethod,
            OccurredAt = now,
            UserId = userId,
            ParkingStayId = stay.Id
        });

        await db.Spots.Where(s => s.Id == stay.SpotId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, SpotStatus.Free));
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "exit", "ParkingStay", stay.Id.ToString(), stay);
        return Results.Ok(StayMapper.ToDto(stay));
    }
}

public sealed record StayDto(
    Guid Id,
    string Plate,
    string VehicleType,
    string SectorCode,
    string SectorName,
    string SpotCode,
    string TariffName,
    decimal FirstHourAmount,
    decimal AdditionalHourAmount,
    DateTimeOffset EntryAt,
    DateTimeOffset? ExitAt,
    int? DurationMinutes,
    decimal? AmountCharged,
    string? PaymentMethod,
    string Status,
    Guid? CashRegisterId,
    string EntryUserName,
    string? ExitUserName);

public static class StayMapper
{
    public static StayDto ToDto(ParkingStay stay) => new(
        stay.Id,
        stay.Plate,
        stay.VehicleType.ToString(),
        stay.SectorCodeSnapshot,
        stay.SectorNameSnapshot,
        stay.SpotCodeSnapshot,
        stay.TariffNameSnapshot,
        stay.FirstHourAmountSnapshot,
        stay.AdditionalHourAmountSnapshot,
        stay.EntryAt,
        stay.ExitAt,
        stay.Duration is null ? null : (int)Math.Ceiling(stay.Duration.Value.TotalMinutes),
        stay.AmountCharged,
        stay.PaymentMethod?.ToString(),
        stay.Status.ToString(),
        stay.CashRegisterId,
        stay.EntryUserId.ToString(),
        stay.ExitUserId?.ToString());
}
