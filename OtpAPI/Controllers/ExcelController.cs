using System.Data;
using System.Drawing;
using Dapper;
using MySqlConnector;               // or MySql.Data.MySqlClient – whichever you already use
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DecorPlast.Excel
{
    public static class OrderSheetBuilder
    {
        private static readonly Color GreyHeader = Color.FromArgb(217, 217, 217);
        private static readonly Color GreyDark = Color.FromArgb(166, 166, 166);
        private static readonly Color GreyNA = Color.FromArgb(191, 191, 191);
        public static readonly Color WhiteNA = Color.FromArgb(255, 255, 255);

        /// <param name="showStock">true = each valid cell shows current stock; false = blank cells to fill in</param>
        /// <param name="codesPerBlock">max code columns per block; longer categories wrap into another block (like DECOR TEX on your sheet)</param>
        public static byte[] Build(SheetHeader header, List<OrderSheetRow> data, bool showStock = false, int codesPerBlock = 35)
        {
            // EPPlus 5–7:
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("DecorPlast");

            var categories = data.GroupBy(d => new { d.CategoryId, d.CategoryName }).OrderBy(g => g.Key.CategoryId).ToList();

            int maxCodes = categories.Max(g => Math.Min(g.Select(x => x.CodeName).Distinct().Count(), codesPerBlock));

            const int firstCol = 2;                    // column A = size labels
            int lastCol = firstCol + maxCodes;         // BUNCH column

            using var pkg = new ExcelPackage();
            var ws = pkg.Workbook.Worksheets.Add("Order Sheet");

            ws.Column(1).Width = 18;
            for (int c = firstCol; c < lastCol; c++) ws.Column(c).Width = 8;
            ws.Column(lastCol).Width = 9;

            BuildTopRow(ws, header, lastCol);
            int row = 2;

            foreach (var cat in categories)
            {
                // order of first appearance (query is ordered by Id)
                var allCodes = cat.Select(x => x.CodeName).Distinct().ToList();

                // (CodeName, Size) -> row
                var map = new Dictionary<(string, string), OrderSheetRow>();
                foreach (var r in cat) map.TryAdd((r.CodeName, r.Size.Trim().ToUpper()), r);

                int blockNo = 0;
                foreach (var chunk in allCodes.Chunk(codesPerBlock))
                {
                    // sizes that exist for at least one code in this block
                    var sizes = cat.Where(x => chunk.Contains(x.CodeName)).Select(x => x.Size.Trim().ToUpper()).Distinct().ToList();

                    // ---- block header row ----
                    var title = ws.Cells[row, 1];
                    title.Value = blockNo == 0 ? cat.Key.CategoryName : cat.Key.CategoryName + " (cont.)";
                    title.Style.Font.Bold = true;
                    title.Style.Font.Size = 8;

                    for (int i = 0; i < chunk.Length; i++)
                    {
                        var c = ws.Cells[row, firstCol + i];
                        c.Value = chunk[i];
                        c.Style.Font.Bold = true;
                        c.Style.Font.Size = 7;
                        c.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        c.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        c.Style.WrapText = true;
                    }
                    var bunch = ws.Cells[row, lastCol];
                    bunch.Value = "BUNCH";
                    bunch.Style.Font.Bold = true;
                    bunch.Style.Font.Size = 7;
                    bunch.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    Fill(ws.Cells[row, 1, row, lastCol], GreyHeader);
                    Borders(ws.Cells[row, 1, row, lastCol]);
                    ws.Row(row).Height = 22;
                    row++;

                    // ---- size rows ----
                    foreach (var size in sizes)
                    {
                        var label = ws.Cells[row, 1];
                        label.Value = size;
                        label.Style.Font.Bold = true;
                        label.Style.Font.Size = 8;
                        label.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        for (int i = 0; i < chunk.Length; i++)
                        {
                            var cell = ws.Cells[row, firstCol + i];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.Font.Size = 8;

                            if (map.TryGetValue((chunk[i], size), out var item))
                            {
                                if (showStock)
                                {
                                    cell.Value = item.Quantity;
                                    if (item.Quantity <= 0)
                                        cell.Style.Font.Color.SetColor(Color.Red);
                                }
                                // else: leave blank for hand / app entry
                            }
                            else
                            {
                                Fill(cell, WhiteNA);   // this code has no such size
                            }
                        }

                        // unused columns to the right of a short block
                        if (chunk.Length < maxCodes)
                            Fill(ws.Cells[row, firstCol + chunk.Length, row, lastCol - 1], GreyHeader);

                        Fill(ws.Cells[row, lastCol], GreyDark);   // BUNCH column
                        Borders(ws.Cells[row, 1, row, lastCol]);
                        ws.Row(row).Height = 15;
                        row++;
                    }

                    row++;      // spacer row
                    blockNo++;
                }
            }

            // ---- view / print ----
            ws.View.FreezePanes(2, 2);
            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            ws.PrinterSettings.PaperSize = ePaperSize.A3;
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;
            ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0.3;
            ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.4;
            ws.PrinterSettings.RepeatRows = ws.Cells["1:1"];

            return pkg.GetAsByteArray();
        }

        // ---- top row: NAME | ADDRESS | VEHICLE NO. | DATE ----
        private static void BuildTopRow(ExcelWorksheet ws, SheetHeader h, int totalCols)
        {
            var parts = new[] { ("NAME", h.Name), ("ADDRESS", h.Address),
                                ("VEHICLE NO.", h.VehicleNo), ("DATE", h.Date) };

            int block = Math.Max(4, totalCols / 4);
            int start = 1;

            for (int i = 0; i < parts.Length; i++)
            {
                int end = (i == parts.Length - 1) ? totalCols : Math.Min(totalCols, start + block - 1);
                int labelEnd = Math.Min(end, start + (i == 0 ? 0 : 2));

                var lbl = ws.Cells[1, start, 1, labelEnd];
                if (labelEnd > start) lbl.Merge = true;
                lbl.Value = parts[i].Item1;
                lbl.Style.Font.Bold = true;
                lbl.Style.Font.Size = 9;
                lbl.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                Fill(lbl, GreyHeader);

                if (labelEnd < end)
                {
                    var val = ws.Cells[1, labelEnd + 1, 1, end];
                    val.Merge = true;
                    val.Value = parts[i].Item2;
                    val.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                Borders(ws.Cells[1, start, 1, end]);
                start = end + 1;
            }
            ws.Row(1).Height = 20;
        }

        private static void Fill(ExcelRange r, Color c)
        {
            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            r.Style.Fill.BackgroundColor.SetColor(c);
        }

        private static void Borders(ExcelRange r)
        {
            r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
    }
}
