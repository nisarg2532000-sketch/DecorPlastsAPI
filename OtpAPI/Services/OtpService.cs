using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OtpAPI.BAL;
using OtpAPI.Data;
using OtpAPI.Models;
using System.Collections.Concurrent;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OtpAPI.Services
{
    public class OtpService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly OtpBAL _otpBAL;
        private static ConcurrentDictionary<string, string> _otpStore = new();

        public OtpService(IConfiguration configuration, ApplicationDbContext context, OtpBAL otpBAL)
        {
            _configuration = configuration;
            _context = context;
            _otpBAL = otpBAL;
        }

        public string GenerateOtp(string phoneNumber)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var otpEntity = new OtpEntity
            {
                PhoneNumber = phoneNumber,
                OtpCode = otp,
                ExpiryTime = DateTime.Now.AddMinutes(5), // ✅ 5 minutes expiry
                IsVerified = false,
                CreatedAt = DateTime.Now
            };

           _= _context.OtpVerifications.Add(otpEntity);
           _= _otpBAL.SaveOtp(otpEntity);

            return otp;
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            var record = _otpBAL.VerifyOtp(phoneNumber, otp);

            return record;
        }

        [HttpPost("send")]
        [EnableRateLimiting("OtpPolicy")]
        public void SendOtp(string phoneNumber, string otp)
        {
            string accountSid = _configuration["Twilio:AccountSid"];
            string authToken = _configuration["Twilio:AuthToken"];
            string fromNumber = _configuration["Twilio:FromNumber"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource.Create(
                body: $"Your OTP is {otp}",
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
    }
}
