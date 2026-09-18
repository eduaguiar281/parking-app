using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Domain;
using ParkingApp.Api.Infrastructure;

namespace ParkingApp.Api.Application.Stays;

public sealed class CancelStay(ParkingDbContext db, AuditWriter audit)
{
    public async Task<IResult> ExecuteAsync(Guid userId, Guid id, string reason)
    {
        var stay = await db.Stays.FindAsync(id);
        if (stay is null || stay.Status != StayStatus.Active)
        {
            return HttpError.Json(409, "not_active", "Só é possível cancelar estadia ativa ainda não paga.");
        }

        stay.Status = StayStatus.Cancelled;
        stay.CancelReason = reason;
        await db.Spots.Where(s => s.Id == stay.SpotId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, SpotStatus.Free));
        await db.SaveChangesAsync();
        await audit.WriteAsync(userId, "cancel", "ParkingStay", stay.Id.ToString(), new { reason, stay.Plate });
        return Results.NoContent();
    }
}
