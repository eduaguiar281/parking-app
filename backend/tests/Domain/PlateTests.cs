using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class PlateTests
{
    [Fact]
    public void Normalize_placaMinuscula_retornaMaiuscula()
    {
        Assert.Equal("ABC-1234", Plate.Normalize("abc-1234"));
    }

    [Fact]
    public void Normalize_placaSemHifen_gravaComHifen()
    {
        Assert.Equal("ABC-1234", Plate.Normalize("abc1234"));
        Assert.True(Plate.IsValid("abc1234"));
    }

    [Fact]
    public void Validate_mercosulValida_aceita()
    {
        Assert.True(Plate.IsValid("abc1d23"));
    }

    [Fact]
    public void Validate_formatoInvalido_rejeita()
    {
        Assert.False(Plate.IsValid("XXXX"));
    }
}
