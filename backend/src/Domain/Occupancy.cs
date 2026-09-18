namespace ParkingApp.Api.Domain;

public static class Occupancy
{
    public static decimal Rate(int occupied, int free)
    {
        var usable = occupied + free;
        if (usable == 0)
        {
            return 0m;
        }

        return (decimal)occupied / usable;
    }
}
