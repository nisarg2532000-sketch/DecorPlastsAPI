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
        public string Status {  get; set; }
        public string Message { get; set; }
    }
}
