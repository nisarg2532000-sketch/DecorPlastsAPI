//// Controllers/ExcelController.cs
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System.ComponentModel;
//using System.Data;
//using System;
//[ApiController]
//[Route("api/[controller]")]
//public class ExcelController : ControllerBase
//{
    // ───────────────────────────────────────────
    // DOWNLOAD — Generate and return an Excel file
    // ───────────────────────────────────────────
//    [HttpGet("download")]
//    public IActionResult DownloadExcel()
//    {
//        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

//        using var package = new ExcelPackage();
//        var sheet = package.Workbook.Worksheets.Add("Orders");

//        // Header row
//        sheet.Cells[1, 1].Value = "OrderId";
//        sheet.Cells[1, 2].Value = "CustomerName";
//        sheet.Cells[1, 3].Value = "Product";
//        sheet.Cells[1, 4].Value = "Quantity";
//        sheet.Cells[1, 5].Value = "Total";

//        // Sample data rows — replace with your DB data
//        var data = GetOrdersFromDb(); // your method
//        for (int i = 0; i < data.Count; i++)
//        {
//            sheet.Cells[i + 2, 1].Value = data[i].OrderId;
//            sheet.Cells[i + 2, 2].Value = data[i].CustomerName;
//            sheet.Cells[i + 2, 3].Value = data[i].Product;
//            sheet.Cells[i + 2, 4].Value = data[i].Quantity;
//            sheet.Cells[i + 2, 5].Value = data[i].Total;
//        }

//        // Auto-fit columns
//        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

//        var fileBytes = package.GetAsByteArray();
//        return File(
//            fileBytes,
//            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//            "Orders.xlsx"
//        );
//    }

//    // ───────────────────────────────────────────
//    // UPLOAD — Read an Excel file and process it
//    // ───────────────────────────────────────────
//    [HttpPost("upload")]
//    public async Task<IActionResult> UploadExcel(IFormFile file)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("No file uploaded.");

//        if (!file.FileName.EndsWith(".xlsx"))
//            return BadRequest("Only .xlsx files are allowed.");

//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//        var results = new List<OrderDto>();

//        using var stream = new MemoryStream();
//        await file.CopyToAsync(stream);

//        using var package = new ExcelPackage(stream);
//        var sheet = package.Workbook.Worksheets[0]; // First sheet
//        int rowCount = sheet.Dimension.Rows;

//        for (int row = 2; row <= rowCount; row++) // row 1 = header
//        {
//            var order = new OrderDto
//            {
//                OrderId = sheet.Cells[row, 1].Text,
//                CustomerName = sheet.Cells[row, 2].Text,
//                Product = sheet.Cells[row, 3].Text,
//                Quantity = int.TryParse(sheet.Cells[row, 4].Text, out int qty) ? qty : 0,
//                Total = decimal.TryParse(sheet.Cells[row, 5].Text, out decimal tot) ? tot : 0
//            };
//            results.Add(order);
//        }

//        // TODO: Save results to your DB here
//        // await _orderService.SaveOrders(results);

//        return Ok(new { message = $"{results.Count} records imported.", data = results });
//    }
//}