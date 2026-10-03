using OtpAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class OrderPdfService
{
    public static byte[] Generate(string orderId, List<OrderListPDF> items)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var first = items.First();

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("DecorPlast - Order Details").FontSize(16).Bold();
                    col.Item().Text($"Order No: {orderId}");
                    col.Item().Text($"Customer: {first.UserName}    Vehicle No: {first.VehicleNo}    Invoice No: {first.InvoiceNo}");
                    col.Item().Text($"Date: {first.CreatedAt:dd-MM-yyyy}");
                    col.Item().PaddingBottom(8).LineHorizontal(1);
                });

                page.Content().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(30);    // #
                        c.RelativeColumn(2);     // Category
                        c.RelativeColumn(2);     // Code
                        c.RelativeColumn(1);     // Qty
                        c.RelativeColumn(1);     // Weight
                    });

                    t.Header(h =>
                    {
                        foreach (var title in new[] { "#", "Category", "Code", "Qty", "Weight" })
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(title).Bold();
                    });

                    int i = 1;
                    foreach (var r in items)
                    {
                        t.Cell().BorderBottom(0.5f).Padding(4).Text(i++.ToString());
                        t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CategoryName);
                        t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CodeName);
                        t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Quantity.ToString());
                        t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Weight.ToString("0.##"));
                    }

                    t.Cell().ColumnSpan(3).Padding(4).AlignRight().Text("Total").Bold();
                    t.Cell().Padding(4).Text(items.Sum(x => x.Quantity).ToString()).Bold();
                    t.Cell().Padding(4).Text(items.Sum(x => x.Weight).ToString("0.##")).Bold();
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page "); x.CurrentPageNumber(); x.Span(" of "); x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}