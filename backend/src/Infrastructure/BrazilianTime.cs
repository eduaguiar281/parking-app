namespace ParkingApp.Api.Infrastructure;

public static class BrazilianTime
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTimeOffset Now() => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone);

    public static DateOnly Today() => DateOnly.FromDateTime(Now().DateTime);

    public static string FormatDate(DateTimeOffset instant) =>
        TimeZoneInfo.ConvertTime(instant, Zone).ToString("dd/MM/yyyy");

    public static string FormatDateTime(DateTimeOffset instant) =>
        TimeZoneInfo.ConvertTime(instant, Zone).ToString("dd/MM/yyyy HH:mm");

    public static string FormatMoney(decimal amount) => amount.ToString("C", new System.Globalization.CultureInfo("pt-BR"));

    private static TimeZoneInfo ResolveZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }
}
