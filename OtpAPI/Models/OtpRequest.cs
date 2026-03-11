namespace OtpAPI.Models
{
    public class OtpRequest
    {
        public string PhoneNumber { get; set; }
    }
    public class VerifyOtpRequest
    {
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
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
    }
    public class AdminDashboard 
    {
        public string Name { get; set; }
        public string PandingOrdercount { get; set; }
        public string CompletedOrderCount { get; set; }
        public string Availavlestockcount { get; set; }
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
        public string Status { get; set; }
    }
    public class GetCode
    {
        public string userid { get; set; }
        public string token { get; set; }
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
    }
    public class GetSize
    {
        public string userid { get; set; }
        public string token { get; set; }
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
        public string SizeId { get; set; }
        public string CategoryId { get; set; }
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
        public string SizeId { get; set; }
        public string CategoryId { get; set; }
    }
    public class SpResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
    public class AddSize
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string Size { get; set; }
    }
    public class DeleteSize
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string SizeId { get; set; }
        public string IsDelete { get; set; }
    }
    public class DeleteCode
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CodeId { get; set; }
        public string IsDelete { get; set; }
    }
    public class DeleteCategory
    {
        public string userid { get; set; }
        public string token { get; set; }
        public string CategoryId { get; set; }
        public string IsDelete { get; set; }
    }
}
