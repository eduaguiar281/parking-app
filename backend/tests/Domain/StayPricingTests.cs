using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class StayPricingTests
{
    [Fact]
    public void Calculate_carro30min_soPrimeiraHora()
    {
        Assert.Equal(10m, StayPricing.Calculate(TimeSpan.FromMinutes(30), 10, 5));
    }

    [Fact]
    public void Calculate_carro1h01_primeiraMaisUma()
    {
        Assert.Equal(15m, StayPricing.Calculate(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(1)), 10, 5));
    }

    [Fact]
    public void Calculate_carro2h30_primeiraMaisDuas()
    {
        Assert.Equal(20m, StayPricing.Calculate(TimeSpan.FromHours(2.5), 10, 5));
    }

    [Fact]
    public void Calculate_moto45min_soPrimeiraHora()
    {
        Assert.Equal(5m, StayPricing.Calculate(TimeSpan.FromMinutes(45), 5, 3));
    }

    [Fact]
    public void Calculate_moto2h10_primeiraMaisDuas()
    {
        Assert.Equal(11m, StayPricing.Calculate(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(10)), 5, 3));
    }
}
