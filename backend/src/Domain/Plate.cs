using System.Text.RegularExpressions;

namespace ParkingApp.Api.Domain;

public static partial class Plate
{
    [GeneratedRegex("^[A-Z]{3}-[0-9]{4}$")]
    private static partial Regex OldFormat();

    [GeneratedRegex("^[A-Z]{3}[0-9]{4}$")]
    private static partial Regex OldFormatWithoutHyphen();

    [GeneratedRegex("^[A-Z]{3}[0-9][A-Z][0-9]{2}$")]
    private static partial Regex MercosulFormat();

    public static string Normalize(string plate)
    {
        var value = plate.Trim().ToUpperInvariant();
        if (OldFormatWithoutHyphen().IsMatch(value))
        {
            return $"{value[..3]}-{value[3..]}";
        }

        return value;
    }

    public static bool IsValid(string plate)
    {
        var normalized = Normalize(plate);
        return OldFormat().IsMatch(normalized) || MercosulFormat().IsMatch(normalized);
    }
}
