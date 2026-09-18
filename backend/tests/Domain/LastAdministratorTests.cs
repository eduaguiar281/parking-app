using ParkingApp.Api.Domain;
using Xunit;

namespace ParkingApp.Api.Tests.Domain;

public class LastAdministratorTests
{
    [Fact]
    public void WouldLeaveNone_unicoAdminAtivo_bloqueia()
    {
        Assert.True(LastAdministrator.WouldLeaveNone(1, true, false));
    }

    [Fact]
    public void WouldLeaveNone_aindaRestaOutroAdmin_permite()
    {
        Assert.False(LastAdministrator.WouldLeaveNone(2, true, false));
    }
}
