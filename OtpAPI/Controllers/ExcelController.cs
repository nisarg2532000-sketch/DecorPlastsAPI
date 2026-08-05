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
    [HttpGet("download")]
    public IActionResult DownloadExcel()
    {
        try
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Stocks");

            // Header row\
            sheet.Cells[1, 1].Value = "CategoryName";
            sheet.Cells[1, 2].Value = "CodeName";
            sheet.Cells[1, 3].Value = "Size";
            sheet.Cells[1, 4].Value = "Weight";
            sheet.Cells[1, 5].Value = "Stock Quantity";

            // Sample data rows — load from BAL
            var data = _apiBAL.ExcelGetStock();
            for (int i = 0; i < data.Count; i++)
            {
                sheet.Cells[i + 2, 1].Value = data[i].Category;
                sheet.Cells[i + 2, 2].Value = data[i].Code;
                sheet.Cells[i + 2, 3].Value = data[i].Size;
                sheet.Cells[i + 2, 4].Value = data[i].Weight;
                sheet.Cells[i + 2, 5].Value = data[i].Quantity;
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
   // [HttpPost("upload")]
    //public async Task<IActionResult> UploadExcel(IFormFile file)
   // {
    //    if (file == null || file.Length == 0)
    //        return BadRequest("No file uploaded.");
//
 //       if (!file.FileName.EndsWith(".xlsx"))
  //          return BadRequest("Only .xlsx files are allowed.");
  //
   //     ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
   //
    //    var results = new List<OrderDto>();
    //
//        using var stream = new MemoryStream();
      //  await file.CopyToAsync(stream);

     //   using var package = new ExcelPackage(stream);
       // var sheet = package.Workbook.Worksheets[0]; // First sheet
       // int rowCount = sheet.Dimension.Rows;

//        for (int row = 2; row <= rowCount; row++) // row 1 = header
  //      {
    //        var order = new OrderDto
      //      {
        //        OrderId = sheet.Cells[row, 1].Text,
          //      CustomerName = sheet.Cells[row, 2].Text,
            //    Product = sheet.Cells[row, 3].Text,
             //   Quantity = int.TryParse(sheet.Cells[row, 4].Text, out int qty) ? qty : 0,
             //   Total = decimal.TryParse(sheet.Cells[row, 5].Text, out decimal tot) ? tot : 0
           // };
           // results.Add(order);
        //}

        // TODO: Save results to your DB here
        // await _orderService.SaveOrders(results);

        //return Ok(new { message = $"{results.Count} records imported.", data = results });
    //}
}