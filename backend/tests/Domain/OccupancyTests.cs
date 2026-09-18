using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class OccupancyTests
{
    [Fact]
    public void OccupancyRate_semVagasUtilizaveis_retornaZero()
    {
        Assert.Equal(0m, Occupancy.Rate(0, 0));
    }
}
