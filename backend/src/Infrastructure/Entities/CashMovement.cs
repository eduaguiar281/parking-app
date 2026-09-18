using ParkingApp.Api.Domain;

namespace ParkingApp.Api.Infrastructure.Entities;

public sealed class CashMovement
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public CashRegister CashRegister { get; set; } = null!;
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParkingStayId { get; set; }
}
