using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class CashExpectedTests
{
    [Fact]
    public void CalculateExpected_aberturaReceitasSangriaAjuste_retornaFormula()
    {
        Assert.Equal(130m, CashExpected.Calculate(100, 50, 10, 20, -10));
    }
}
