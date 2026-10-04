using OtpAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class OrderPdfService
{
    public static byte[] Generate(List<OrderListPDF> items)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var orders = items.GroupBy(x => x.OrderId).ToList();

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
                    col.Item().PaddingBottom(8).LineHorizontal(1);
                });

                page.Content().Column(main =>
                {
                    main.Spacing(14);

                    foreach (var g in orders)
                    {
                        var first = g.First();

                        main.Item().Column(sec =>
                        {
                            sec.Item().Text($"Order No: {g.Key}").Bold().FontSize(12);
                            sec.Item().Text($"Customer: {first.UserName}    Vehicle No: {first.VehicleNo}    Invoice No: {first.InvoiceNo}    Date: {first.CreatedAt:dd-MM-yyyy}");

                            sec.Item().PaddingTop(4).Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(30);
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(1);
                                    c.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    foreach (var title in new[] { "#", "Category", "Code", "Qty", "Weight" })
                                        h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(title).Bold();
                                });

                                int i = 1;
                                foreach (var r in g)
                                {
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text((i++).ToString());
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CategoryName);
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CodeName);
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Quantity.ToString());
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Weight.ToString("0.##"));
                                }

                                t.Cell().ColumnSpan(3).Padding(4).AlignRight().Text("Order Total").Bold();
                                t.Cell().Padding(4).Text(g.Sum(x => x.Quantity).ToString()).Bold();
                                t.Cell().Padding(4).Text(g.Sum(x => x.Weight).ToString("0.##")).Bold();
                            });
                        });
                    }

                    if (orders.Count > 1)
                    {
                        main.Item().AlignRight().Text(
                            $"Grand Total - Qty: {items.Sum(x => x.Quantity)}    Weight: {items.Sum(x => x.Weight):0.##}")
                            .Bold().FontSize(12);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page "); x.CurrentPageNumber(); x.Span(" of "); x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}