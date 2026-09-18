using System.Text.Json;
using System.Text.Json.Serialization;
using ParkingApp.Api.Infrastructure.Entities;

namespace ParkingApp.Api.Infrastructure;

public sealed class AuditWriter(ParkingDbContext db)
{
    private static readonly JsonSerializerOptions Json = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public async Task WriteAsync(
        Guid userId,
        string action,
        string entityType,
        string entityId,
        object changedData,
        CancellationToken cancellationToken = default)
    {
        db.AuditEvents.Add(new AuditEvent
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            ChangedData = JsonSerializer.Serialize(changedData, Json)
        });
        await db.SaveChangesAsync(cancellationToken);
    }
}
