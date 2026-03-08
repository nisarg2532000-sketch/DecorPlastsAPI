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
        public string AdminName { get; set; }
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
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
    }
}
