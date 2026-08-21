using OtpAPI.Models;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace OtpAPI.Models
{
    public class SpResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
    public class SpResultcode
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int CodeId { get; set; } = 0;
    }
    public class APIRequest
    {
        public string PhoneNumber { get; set; }
    }
    public class VerifyOtpRequest
    {
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
    }
    public class OtpResult
    {
        public string Status { get; set; } = "";
        public string Details { get; set; } = "";
        public string OTP { get; set; } = "";
    }
    public class OtpEntity
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class IsverifyOtp
    {
        public bool Status {  get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
    }
    public class InsertUpdateUser
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string Id { get; set; }
        public string OwnerName { get; set; }
        public string ShopName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string IsActive { get; set; }
    }
    public class UserData
    {
        public string Id { get; set; }
        public string OwnerName { get; set; }
        public string ShopName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string IsActive { get; set; }
    }
    public class AdminDashboard 
    {
        public string totalOrdersCount { get; set; }
        public string FutureOrdersCount { get; set; }
        public string AvailableStockCount { get; set; }
        public string totalUsercount { get; set; }
    }
    public class getdata
    {
        public string userid { get; set; }
        public string token { get; set; }
    }
    public class GetCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
    }
    public class GetCodeByCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<GetCode> Codes { get; set; }
    }
    public class GetCode
    {
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public bool Status { get; set; }
        public string Size { get; set; } 
        public int Quantity { get; set; }
        public decimal Weight { get; set; }

    }
    public class GetCodeRaw
    {
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }  
        public string Size { get; set; } 
        public int Quantity { get; set; }
        public float Weight { get; set; }
        public bool Status { get; set; }
    }

    public class SizeItem
    {
        public string Size { get; set; }
        public int Quantity { get; set; }
        public float Weight { get; set; }
    }
    public class GetSize
    {
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
    }
    public class UpdateCategory
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; }
    }
    public class UpdateCodes
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get;set; }
        public string Size { get; set; }
        public string CategoryId { get; set; }
        public float Weight{ get; set; }
        public string Status { get; set; }
    }
    public class UpdateSize
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string SizeId { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
    }
    public class AddCategory
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string Category { get; set; }
    }
    public class AddCode
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CodeName { get; set; }
        public string Size { get; set; }
        public string CategoryId { get; set; }
        public string Weight { get; set; }
        public int? Quantity { get; set; }
    }
    public class AddSize
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string Size { get; set; }
    }
    public class Delete
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string Id { get; set; }
    }
    public class AddUpdateStock
    {
        public string userid { get; set; }
        public string token { get; set; }
        public List<Stockitem> Stockitem { get; set; }
    }
    public class Stockitem
    {
        public string CategoryId { get; set; }
        public string CodeId { get; set; }
        public string Quantity { get; set; }
    }
    public class GetStock
    {
        public string id { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
    }
    public class OrderuserItem
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
    }
    public class OrderItem
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
        public float Weight { get; set; }
    }
    public class StockCheckResult
    {
        public bool IsAvailable { get; set; }
        public int AvailableStock { get; set; }
    }
    public class GetOrderList
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string MobileNo { get; set; }
        public string OrderId { get; set; }
        public List<OrderItem> items { get; set; }
        public string Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string DateTime { get; set; }
    }
    public class GetOrderListDemo
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string MobileNo { get; set; }
        public string OrderId { get; set; }
        public List<OrderItem> items { get; set; }
        public string Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string DateTime { get; set; }
        public string? UpdatedDatetime { get; set; }
    }
    public class GetUserOrder
    {
        public string userid { get; set; }
        public string OrderId { get; set; }
        public List<OrderuserItem> items { get; set; }
        public string Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string DateTime { get; set; }
    }
    public class GetFutureOrderList
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string MobileNo { get; set; }
        public string OrderId { get; set; }
        public List<FutureOrderItem> items { get; set; }
        public string Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string DateTime { get; set; }
    }
    public class GetFutureOrderListDemo
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string MobileNo { get; set; }
        public string OrderId { get; set; }
        public List<FutureOrderItem> items { get; set; }
        public string Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string DateTime { get; set; }
        public string? UpdatedDateTime { get; set; } 
    }
    public class FutureOrderItem
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get; set; }
        public string Size { get; set; }
        public string RemainQuantity { get; set; }
        public string TotalQuantity { get; set; }
        public float Weight { get; set; }
    }
    public class InsertOrder
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string OrderId { get; set; }
        public List<InsertOrderitem> items { get; set; }
        public bool Status { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
    }
    public class InsertOrderitem
    {
        public string CategoryId { get; set; }
        public string CodeId { get; set; }
        public string Quantity { get; set; }

    }
    public class UpdateOrder
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string OrderId { get; set; }
        public string? VehicleNo { get; set; }
        public string? InvoiceNo { get; set; }
        public int Status { get; set; }
    }

    public class GetTotalWeightByUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TotalWeight { get; set; }
    }
    public class InsertUpdateCart
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CartId { get; set; }
        public string CategoryId { get; set; }
        public string CodeId { get; set; }
        public string Quantity { get; set; }
    }
    public class GetCart
    {
        public int id { get; set; }
        public int categoryid { get; set; }
        public string CategoryName { get; set; }
        public int codeid { get; set; }
        public string CodeName { get; set; }
        public int sizeid { get; set; }
        public string Size { get; set; }
        public int quantity { get; set; }
        public DateTime createddate { get; set; }
    }
    public class Notifications
    {
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class Count
    {
        public string NotificationCount { get; set; }
    }
    public class AppVersionResponse
    {
        public string LatestVersion { get; set; } = "";
        public bool ForceUpdate { get; set; }
 
        public string Message { get; set; } = "";
    }
    public class ExcelGetStock
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        // Quantity as integer for processing
        public int Quantity { get; set; }
    }
}
public class InsertOrderbyAdmin
{
    public string AdminId { get; set; }
    public string token { get; set; }
    public string userid { get; set; }
    public string OrderId { get; set; }
    public List<InsertOrderitem> items { get; set; }
    public bool Status { get; set; }
    public string? VehicleNo { get; set; }
    public string? InvoiceNo { get; set; }
}