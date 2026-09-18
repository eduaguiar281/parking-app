using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Application.Cash;
using ParkingApp.Api.Application.Stays;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;

namespace ParkingApp.Api.Application.Reports;

public sealed class ReportService(ParkingDbContext db)
{
    public async Task<object> CashAsync(DateOnly from, DateOnly to)
    {
        var items = await db.CashRegisters
            .Include(c => c.Movements)
            .Where(c => c.OperationalDate >= from && c.OperationalDate <= to)
            .OrderBy(c => c.OperationalDate)
            .ToListAsync();
        return new { items = items.Select(CashService.ToDto) };
    }

    public async Task<object> StaysAsync(DateOnly from, DateOnly to)
    {
        var start = new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        var end = new DateTimeOffset(to.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), TimeSpan.Zero);
        var stays = (await db.Stays.ToListAsync())
            .Where(s => s.EntryAt >= start && s.EntryAt <= end)
            .OrderBy(s => s.EntryAt)
            .ToList();
        var completed = stays.Where(s => s.Status == StayStatus.Completed).ToList();
        var currentlyParked = await db.Stays.CountAsync(s => s.Status == StayStatus.Active);
        var spots = await db.Spots.Include(s => s.Sector).ToListAsync();
        return new
        {
            stays = stays.Select(StayMapper.ToDto).ToList(),
            entryCount = stays.Count,
            exitCount = completed.Count,
            currentlyParked,
            totalRevenue = completed.Sum(s => s.AmountCharged ?? 0),
            revenueByVehicleType = completed
                .GroupBy(s => s.VehicleType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(s => s.AmountCharged ?? 0)),
            revenueBySector = completed
                .GroupBy(s => s.SectorCodeSnapshot)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.AmountCharged ?? 0)),
            averageStayMinutes = completed.Count == 0
                ? 0
                : completed.Average(s => s.Duration?.TotalMinutes ?? 0),
            occupancyBySector = spots
                .Where(s => s.Sector is { Status: SectorStatus.Active })
                .GroupBy(s => s.Sector.Code)
                .ToDictionary(
                    g => g.Key,
                    g => Occupancy.Rate(
                        g.Count(s => s.Status == SpotStatus.Occupied),
                        g.Count(s => s.Status == SpotStatus.Free)))
        };
    }
}
