using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OtpAPI.BAL;
using OtpAPI.Models;


[ApiController]
[Route("api/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly APIBAL _apiBAL;

    public ExcelController(APIBAL apiBAL)
    {
        _apiBAL = apiBAL;
    }
    // ───────────────────────────────────────────
    // DOWNLOAD — Generate and return an Excel file
    // ───────────────────────────────────────────
    [HttpGet("DownloadExcel")]
    public IActionResult DownloadExcel()
    {
        try
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Stocks");

            // Header row
            sheet.Cells[1, 1].Value = "CategoryId";
            sheet.Cells[1, 2].Value = "CategoryName";
            sheet.Cells[1, 3].Value = "CodeId";
            sheet.Cells[1, 4].Value = "CodeName";
            sheet.Cells[1, 5].Value = "Size";
            sheet.Cells[1, 6].Value = "Weight";
            sheet.Cells[1, 7].Value = "Stock Quantity";
            // Sample data rows — load from BAL
            var data = _apiBAL.ExcelGetStock();
            for (int i = 0; i < data.Count; i++)
            {
                sheet.Cells[i + 2, 1].Value = data[i].CategoryId;
                sheet.Cells[i + 2, 2].Value = data[i].Category;
                sheet.Cells[i + 2, 3].Value = data[i].CodeId;
                sheet.Cells[i + 2, 4].Value = data[i].Code;
                sheet.Cells[i + 2, 5].Value = data[i].Size;
                sheet.Cells[i + 2, 6].Value = data[i].Weight;
                sheet.Cells[i + 2, 7].Value = data[i].Quantity;
            }

            // Auto-fit columns
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            var fileBytes = package.GetAsByteArray();
            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Stock.xlsx"
            );
        }
        catch (Exception ex)
        {
            return BadRequest(StatusCode(500, new { Message = "An error occurred while Downloading excel", Details = ex.Message }));
        }
    }

    // ───────────────────────────────────────────
    // UPLOAD — Read an Excel file and process it
    // ───────────────────────────────────────────
    [HttpPost("UploadExcel")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx files are allowed.");


        var results = new List<ExcelGetStock>();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var sheet = package.Workbook.Worksheets[0]; // First sheet
        int rowCount = sheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++) // row 1 = header
        {
            var Stock = new ExcelGetStock
            {
                CategoryId = sheet.Cells[row, 1].Text,
                CodeId = sheet.Cells[row, 3].Text,
                Weight = sheet.Cells[row, 6].Text,
                Quantity = int.TryParse(sheet.Cells[row, 7].Text, out int qty) ? qty : 0
            };
            results.Add(Stock);
        }

        // TODO: Save results to your DB here (synchronous)
        var saved = _apiBAL.SaveStock(results);

        return Ok(new { message = $"{results.Count} records imported.", savedRows = saved });
    }
}