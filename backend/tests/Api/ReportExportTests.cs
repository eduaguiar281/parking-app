using System.Net;
using Xunit;

namespace ParkingApp.Api.Tests.Api;

public class ReportExportTests : ApiTest
{
    [Fact]
    public async Task GetReportPdf_admin_retornaApplicationPdf()
    {
        var client = await AdminClient();
        var response = await client.GetAsync("/api/reports/cash.pdf?from=2026-01-01&to=2026-12-31");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
    }
}
