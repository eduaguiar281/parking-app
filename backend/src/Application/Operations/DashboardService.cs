using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;

namespace ParkingApp.Api.Application.Operations;

public sealed class DashboardService(ParkingDbContext db)
{
    public async Task<object> GetAsync()
    {
        var spots = await db.Spots.Include(s => s.Sector).ToListAsync();
        var total = spots.Count;
        var occupied = spots.Count(s => s.Status == SpotStatus.Occupied);
        var free = spots.Count(s => s.Status == SpotStatus.Free && s.Sector is { Status: SectorStatus.Active });
        var blocked = spots.Count(s => s.Status == SpotStatus.Blocked);
        var maintenance = spots.Count(s => s.Status == SpotStatus.Maintenance);
        var occupancyBySector = spots
            .Where(s => s.Sector is { Status: SectorStatus.Active })
            .GroupBy(s => s.Sector.Code)
            .Select(g => new
            {
                sectorCode = g.Key,
                occupancyRate = Occupancy.Rate(
                    g.Count(s => s.Status == SpotStatus.Occupied),
                    g.Count(s => s.Status == SpotStatus.Free))
            })
            .ToList();

        var cash = await db.CashRegisters.Include(c => c.Movements)
            .FirstOrDefaultAsync(c => c.Status == CashStatus.Open);
        var dayRevenue = cash?.Movements
            .Where(m => m.Type == CashMovementType.ExitPayment)
            .Sum(m => m.Amount) ?? 0m;

        var parked = (await db.Stays
            .Where(s => s.Status == StayStatus.Active)
            .ToListAsync())
            .OrderBy(s => s.EntryAt)
            .ToList();

        return new
        {
            totalSpots = total,
            occupied,
            free,
            blocked,
            maintenance,
            occupancyRate = Occupancy.Rate(occupied, free),
            occupancyBySector,
            cashOpen = cash is not null,
            cashOperationalDate = cash?.OperationalDate,
            dayRevenue,
            parked = parked.Select(Stays.StayMapper.ToDto).ToList()
        };
    }
}
