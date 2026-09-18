namespace ParkingApp.Api.Domain;

public static class LastAdministrator
{
    public static bool WouldLeaveNone(
        int activeAdministratorCount,
        bool targetIsActiveAdministrator,
        bool targetRemainsActiveAdministrator) =>
        targetIsActiveAdministrator &&
        !targetRemainsActiveAdministrator &&
        activeAdministratorCount <= 1;
}
