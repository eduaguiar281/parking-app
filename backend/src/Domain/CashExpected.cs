namespace ParkingApp.Api.Domain;

public static class CashExpected
{
    public static decimal Calculate(
        decimal openingAmount,
        decimal exitPayments,
        decimal supplies,
        decimal bleeds,
        decimal adjustments) =>
        openingAmount + exitPayments + supplies - bleeds + adjustments;
}
