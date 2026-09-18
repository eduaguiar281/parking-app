using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Application.Stays;

public sealed class StayHistory(ParkingDbContext db)
{
    public async Task<IResult> ListAsync(
        DateOnly? from,
        DateOnly? to,
        string? plate,
        Guid? sectorId,
        Guid? spotId,
        VehicleType? vehicleType,
        PaymentMethod? paymentMethod,
        Guid? operatorUserId,
        StayStatus? status)
    {
        var stays = await db.Stays.ToListAsync();
        IEnumerable<ParkingStay> query = stays;
        if (from is not null)
        {
            var start = new DateTimeOffset(from.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
            query = query.Where(s => s.EntryAt >= start);
        }

        if (to is not null)
        {
            var end = new DateTimeOffset(to.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), TimeSpan.Zero);
            query = query.Where(s => s.EntryAt <= end);
        }

        if (!string.IsNullOrWhiteSpace(plate))
        {
            var normalized = Plate.Normalize(plate);
            query = query.Where(s => s.Plate.Contains(normalized));
        }

        if (sectorId is not null)
        {
            query = query.Where(s => s.SectorId == sectorId);
        }

        if (spotId is not null)
        {
            query = query.Where(s => s.SpotId == spotId);
        }

        if (vehicleType is not null)
        {
            query = query.Where(s => s.VehicleType == vehicleType);
        }

        if (paymentMethod is not null)
        {
            query = query.Where(s => s.PaymentMethod == paymentMethod);
        }

        if (operatorUserId is not null)
        {
            query = query.Where(s => s.EntryUserId == operatorUserId || s.ExitUserId == operatorUserId);
        }

        if (status is not null)
        {
            query = query.Where(s => s.Status == status);
        }

        return Results.Ok(query.OrderByDescending(s => s.EntryAt).Select(StayMapper.ToDto).ToList());
    }
}
