using System.Text;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ParkingApp.Api.Infrastructure;

public static class ReportExport
{
    static ReportExport()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] ToCsv(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return Encoding.UTF8.GetBytes(json);
    }

    public static byte[] ToPdf(string title, object payload)
    {
        var body = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Header().Text(title).FontSize(18).Bold();
                page.Content().Text(body).FontSize(10);
            });
        }).GeneratePdf();
    }
}
