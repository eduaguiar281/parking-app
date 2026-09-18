using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Cash;

public sealed class CashService(ParkingDbContext db, AuditWriter audit)
{
    public async Task<IResult> CurrentAsync()
    {
        var cash = await db.CashRegisters
            .Include(c => c.Movements)
            .FirstOrDefaultAsync(c => c.Status == CashStatus.Open)
            ?? await db.CashRegisters.Include(c => c.Movements)
                .OrderByDescending(c => c.OperationalDate)
                .FirstOrDefaultAsync();
        return cash is null
            ? HttpError.Json(404, "no_open_cash", "Nenhum caixa aberto.")
            : Results.Ok(ToDto(cash));
    }

    public async Task<IResult> GetAsync(Guid id)
    {
        var cash = await db.CashRegisters.Include(c => c.Movements).FirstOrDefaultAsync(c => c.Id == id);
        return cash is null
            ? HttpError.Json(404, "not_found", "Caixa não encontrado.")
            : Results.Ok(ToDto(cash));
    }

    public async Task<IResult> OpenAsync(Guid userId, DateOnly operationalDate, decimal openingAmount)
    {
        if (await db.CashRegisters.AnyAsync(c => c.Status == CashStatus.Open))
        {
            return HttpError.Json(409, "cash_open", "Já existe um caixa aberto. Feche-o antes de abrir outro.");
        }

        if (await db.CashRegisters.AnyAsync(c => c.OperationalDate == operationalDate))
        {
            return HttpError.Json(409, "cash_exists", "Já existe caixa para esta data operacional.");
        }

        var cash = new CashRegister
        {
            Id = Guid.NewGuid(),
            OperationalDate = operationalDate,
            Status = CashStatus.Open,
            OpeningAmount = openingAmount,
            OpenedAt = DateTimeOffset.UtcNow,
            OpenedByUserId = userId
        };
        db.CashRegisters.Add(cash);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "open", "CashRegister", cash.Id.ToString(), cash);
        return Results.Created($"/api/cash/{cash.Id}", ToDto(cash));
    }

    public async Task<IResult> CloseAsync(Guid userId, Guid id, decimal informedAmount, string? notes)
    {
        var cash = await db.CashRegisters.Include(c => c.Movements).FirstOrDefaultAsync(c => c.Id == id);
        if (cash is null)
        {
            return HttpError.Json(404, "not_found", "Caixa não encontrado.");
        }

        if (cash.Status != CashStatus.Open)
        {
            return HttpError.Json(409, "cash_closed", "Caixa já está fechado.");
        }

        var expected = Expected(cash);
        cash.Status = CashStatus.Closed;
        cash.InformedClosingAmount = informedAmount;
        cash.ExpectedAmount = expected;
        cash.Difference = informedAmount - expected;
        cash.ClosedAt = DateTimeOffset.UtcNow;
        cash.ClosedByUserId = userId;
        cash.ClosingNotes = notes;
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "close", "CashRegister", cash.Id.ToString(), cash);
        return Results.Ok(ToDto(cash));
    }

    public async Task<IResult> ReopenAsync(Guid userId, Guid id, string reason)
    {
        if (await db.CashRegisters.AnyAsync(c => c.Status == CashStatus.Open))
        {
            return HttpError.Json(409, "cash_open", "Já existe outro caixa aberto.");
        }

        var cash = await db.CashRegisters.Include(c => c.Movements).FirstOrDefaultAsync(c => c.Id == id);
        if (cash is null)
        {
            return HttpError.Json(404, "not_found", "Caixa não encontrado.");
        }

        cash.Status = CashStatus.Open;
        cash.ReopenReason = reason;
        cash.ClosedAt = null;
        cash.ClosedByUserId = null;
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "reopen", "CashRegister", cash.Id.ToString(), new { reason });
        return Results.Ok(ToDto(cash));
    }

    public async Task<IResult> AddMovementAsync(Guid userId, Guid id, CashMovementType type, decimal amount, string reason)
    {
        if (type is not (CashMovementType.Bleed or CashMovementType.Supply or CashMovementType.Adjustment))
        {
            return HttpError.Json(400, "invalid_type", "Tipo de movimento não permitido.");
        }

        var cash = await db.CashRegisters.Include(c => c.Movements).FirstOrDefaultAsync(c => c.Id == id);
        if (cash is null)
        {
            return HttpError.Json(404, "not_found", "Caixa não encontrado.");
        }

        if (cash.Status != CashStatus.Open)
        {
            return HttpError.Json(409, "cash_closed", "Caixa fechado não recebe movimentos. Reabra e lance sangria, suprimento ou ajuste.");
        }

        var movement = new CashMovement
        {
            Id = Guid.NewGuid(),
            CashRegisterId = cash.Id,
            Type = type,
            Amount = amount,
            Reason = reason,
            OccurredAt = DateTimeOffset.UtcNow,
            UserId = userId
        };
        db.CashMovements.Add(movement);
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, type.ToString().ToLowerInvariant(), "CashMovement", movement.Id.ToString(), movement);
        return Results.Created($"/api/cash/{id}/movements", movement);
    }

    public static decimal Expected(CashRegister cash)
    {
        var exits = cash.Movements.Where(m => m.Type == CashMovementType.ExitPayment).Sum(m => m.Amount);
        var supplies = cash.Movements.Where(m => m.Type == CashMovementType.Supply).Sum(m => m.Amount);
        var bleeds = cash.Movements.Where(m => m.Type == CashMovementType.Bleed).Sum(m => m.Amount);
        var adjustments = cash.Movements.Where(m => m.Type == CashMovementType.Adjustment).Sum(m => m.Amount);
        return CashExpected.Calculate(cash.OpeningAmount, exits, supplies, bleeds, adjustments);
    }

    public static object ToDto(CashRegister cash)
    {
        var totals = cash.Movements
            .Where(m => m.Type == CashMovementType.ExitPayment && m.PaymentMethod is not null)
            .GroupBy(m => m.PaymentMethod!.Value)
            .ToDictionary(g => g.Key.ToString(), g => g.Sum(m => m.Amount));

        return new
        {
            id = cash.Id,
            operationalDate = cash.OperationalDate,
            status = cash.Status.ToString(),
            openingAmount = cash.OpeningAmount,
            expectedAmount = Expected(cash),
            informedClosingAmount = cash.InformedClosingAmount,
            difference = cash.Difference,
            totalsByPaymentMethod = totals,
            movements = cash.Movements.OrderBy(m => m.OccurredAt).Select(m => new
            {
                m.Id,
                type = m.Type.ToString(),
                m.Amount,
                paymentMethod = m.PaymentMethod?.ToString(),
                m.Reason,
                m.OccurredAt
            }),
            openedByUserId = cash.OpenedByUserId,
            closedByUserId = cash.ClosedByUserId,
            closingNotes = cash.ClosingNotes
        };
    }
}
