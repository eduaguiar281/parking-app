namespace ParkingApp.Api.Domain;

public static class StayPricing
{
    public static decimal Calculate(TimeSpan duration, decimal firstHourAmount, decimal additionalHourAmount)
    {
        if (duration <= TimeSpan.FromHours(1))
        {
            return firstHourAmount;
        }

        var extraHours = duration.TotalHours - 1;
        var additionalHours = (int)Math.Ceiling(extraHours);
        return firstHourAmount + additionalHourAmount * additionalHours;
    }
}
