using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Infrastructure.Entities;

public sealed class CashRegister
{
    public Guid Id { get; set; }
    public DateOnly OperationalDate { get; set; }
    public CashStatus Status { get; set; }
    public decimal OpeningAmount { get; set; }
    public DateTimeOffset OpenedAt { get; set; }
    public Guid OpenedByUserId { get; set; }
    public decimal? InformedClosingAmount { get; set; }
    public decimal? ExpectedAmount { get; set; }
    public decimal? Difference { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public Guid? ClosedByUserId { get; set; }
    public string? ClosingNotes { get; set; }
    public string? ReopenReason { get; set; }
    public List<CashMovement> Movements { get; set; } = [];
}
