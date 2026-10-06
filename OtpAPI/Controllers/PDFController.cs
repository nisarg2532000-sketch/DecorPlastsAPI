using OtpAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class OrderPdfService
{
    public static byte[] Generate(List<OrderListPDF> items)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var shopname = items.GroupBy(x => x.ShopName).ToList();

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("DecorPlast - Vehicle Details").FontSize(16).Bold();
                    col.Item().PaddingBottom(8).LineHorizontal(1);
                });

                page.Content().Column(main =>
                {
                    main.Spacing(14);

                    foreach (var g in shopname)
                    {
                        var first = g.First();

                        main.Item().Column(sec =>
                        {
                            sec.Item().Text($"Customer: {first.ShopName}").Bold().FontSize(12);
                            sec.Item().Text($"Vehicle No: {first.VehicleNo}    Date: {first.CreatedAt:dd-MM-yyyy}");

                            sec.Item().PaddingTop(4).Table(t =>
                            {
                                t.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(30);
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(1);
                                    c.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    foreach (var title in new[] { "No.", "Category", "Code", "Size", "Qty", "Weight" })
                                        h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(title).Bold();
                                });

                                int i = 1;
                                foreach (var r in g)
                                {
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text((i++).ToString());
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CategoryName);
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.CodeName);
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Size);
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Quantity.ToString());
                                    t.Cell().BorderBottom(0.5f).Padding(4).Text(r.Weight.ToString("0.##"));
                                }

                                t.Cell().ColumnSpan(4).BorderTop(1).Padding(4).AlignRight().Text("Order Total").Bold();
                                t.Cell().BorderTop(1).Padding(4).Text(g.Sum(x => x.Quantity).ToString()).Bold();
                                t.Cell().BorderTop(1).Padding(4).Text(g.Sum(x => x.Weight).ToString("0.##")).Bold();
                            });
                        });
                    }

                    if (shopname.Count > 1)
                    {
                        main.Item().AlignRight().Text(
                            $"Grand Total - Qty: {items.Sum(x => x.Quantity)}   Total Weight: {items.Sum(x => x.Weight):0.##}")
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