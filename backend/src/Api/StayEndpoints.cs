using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Application.Cash;
using ParkingApp.Api.Application.Operations;
using ParkingApp.Api.Application.Reports;
using ParkingApp.Api.Application.Stays;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;

namespace ParkingApp.Api.Api;

public static class OperationEndpoints
{
    public static RouteGroupBuilder MapOperations(this RouteGroupBuilder api)
    {
        api.MapGet("/operations/dashboard", async (DashboardService dashboard) =>
            Results.Ok(await dashboard.GetAsync()));

        api.MapPost("/stays", async (HttpContext http, RegisterEntry entry, EntryWrite body) =>
            await entry.ExecuteAsync(http.User.UserId(), body));
        api.MapGet("/stays", async (
            StayHistory history,
            DateOnly? from,
            DateOnly? to,
            string? plate,
            Guid? sectorId,
            Guid? spotId,
            VehicleType? vehicleType,
            PaymentMethod? paymentMethod,
            Guid? operatorUserId,
            StayStatus? status) =>
            await history.ListAsync(from, to, plate, sectorId, spotId, vehicleType, paymentMethod, operatorUserId, status));
        api.MapGet("/stays/{id:guid}/exit-preview", async (ExitStay exit, Guid id) => await exit.PreviewAsync(id));
        api.MapPost("/stays/{id:guid}/exit", async (HttpContext http, ExitStay exit, Guid id, ExitWrite body) =>
            await exit.ConfirmAsync(http.User.UserId(), id, body.PaymentMethod));
        api.MapPost("/stays/{id:guid}/cancel", async (HttpContext http, CancelStay cancel, Guid id, CancelWrite body) =>
            http.User.AdminOnly() ?? await cancel.ExecuteAsync(http.User.UserId(), id, body.Reason));
        api.MapGet("/stays/{id:guid}/receipt", async (ParkingDbContext db, Guid id) =>
        {
            var stay = await db.Stays.FindAsync(id);
            return stay is null
                ? HttpError.Json(404, "not_found", "Estadia não encontrada.")
                : Results.Ok(StayMapper.ToDto(stay));
        });

        api.MapGet("/cash/current", async (CashService cash) => await cash.CurrentAsync());
        api.MapPost("/cash", async (HttpContext http, CashService cash, OpenCashWrite body) =>
            await cash.OpenAsync(http.User.UserId(), body.OperationalDate, body.OpeningAmount));
        api.MapGet("/cash/{id:guid}", async (CashService cash, Guid id) => await cash.GetAsync(id));
        api.MapPost("/cash/{id:guid}/close", async (HttpContext http, CashService cash, Guid id, CloseCashWrite body) =>
            await cash.CloseAsync(http.User.UserId(), id, body.InformedAmount, body.Notes));
        api.MapPost("/cash/{id:guid}/reopen", async (HttpContext http, CashService cash, Guid id, ReopenWrite body) =>
            http.User.AdminOnly() ?? await cash.ReopenAsync(http.User.UserId(), id, body.Reason));
        api.MapPost("/cash/{id:guid}/movements", async (HttpContext http, CashService cash, Guid id, MovementWrite body) =>
            http.User.AdminOnly() ?? await cash.AddMovementAsync(http.User.UserId(), id, body.Type, body.Amount, body.Reason));

        api.MapGet("/reports/cash", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
            http.User.AdminOnly() ?? Results.Ok(await reports.CashAsync(from, to)));
        api.MapGet("/reports/stays", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
            http.User.AdminOnly() ?? Results.Ok(await reports.StaysAsync(from, to)));
        api.MapGet("/reports/cash.pdf", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
        {
            var denied = http.User.AdminOnly();
            if (denied is not null) return denied;
            var payload = await reports.CashAsync(from, to);
            return Results.File(ReportExport.ToPdf("Relatório de caixa", payload), "application/pdf", "caixa.pdf");
        });
        api.MapGet("/reports/cash.csv", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
        {
            var denied = http.User.AdminOnly();
            if (denied is not null) return denied;
            var payload = await reports.CashAsync(from, to);
            return Results.File(ReportExport.ToCsv(payload), "text/csv", "caixa.csv");
        });
        api.MapGet("/reports/stays.pdf", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
        {
            var denied = http.User.AdminOnly();
            if (denied is not null) return denied;
            var payload = await reports.StaysAsync(from, to);
            return Results.File(ReportExport.ToPdf("Relatório de estadias", payload), "application/pdf", "estadias.pdf");
        });
        api.MapGet("/reports/stays.csv", async (HttpContext http, ReportService reports, DateOnly from, DateOnly to) =>
        {
            var denied = http.User.AdminOnly();
            if (denied is not null) return denied;
            var payload = await reports.StaysAsync(from, to);
            return Results.File(ReportExport.ToCsv(payload), "text/csv", "estadias.csv");
        });

        api.MapGet("/audit", async (HttpContext http, ParkingDbContext db, DateOnly? from, DateOnly? to) =>
        {
            var denied = http.User.AdminOnly();
            if (denied is not null) return denied;
            var users = (await db.Users.ToListAsync()).ToDictionary(u => u.Id, u => u.FullName);
            var events = (await db.AuditEvents.ToListAsync())
                .Where(a => from is null || a.OccurredAt >= new DateTimeOffset(from.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero))
                .Where(a => to is null || a.OccurredAt <= new DateTimeOffset(to.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), TimeSpan.Zero))
                .OrderByDescending(a => a.OccurredAt)
                .Select(a => new
            {
                a.Id,
                a.OccurredAt,
                userName = users.GetValueOrDefault(a.UserId, a.UserId.ToString()),
                a.Action,
                a.EntityType,
                a.EntityId,
                changedData = a.ChangedData
            });
            return Results.Ok(events.ToList());
        });

        return api;
    }
}

public sealed record ExitWrite(PaymentMethod PaymentMethod);
public sealed record CancelWrite(string Reason);
public sealed record OpenCashWrite(DateOnly OperationalDate, decimal OpeningAmount);
public sealed record CloseCashWrite(decimal InformedAmount, string? Notes);
public sealed record ReopenWrite(string Reason);
public sealed record MovementWrite(CashMovementType Type, decimal Amount, string Reason);
