using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OtpAPI.BAL;
using OtpAPI.Data;
using OtpAPI.Models;
using System.Collections.Concurrent;
using System.Text.Json;
using Twilio;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using static System.Net.WebRequestMethods;

namespace OtpAPI.Services
{
    public class OtpService
    {
        private readonly APIBAL _otpBAL;
        private readonly string _apiKey;
        private readonly System.Net.Http.HttpClient _httpClient;
        private static ConcurrentDictionary<string, string> _otpStore = new();

        public OtpService(IConfiguration configuration, APIBAL otpBAL, HttpClient httpClient)
        {
            _otpBAL = otpBAL;
            _httpClient = httpClient;
            _apiKey = configuration["TwoFactor:ApiKey"];
            
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            var record = _otpBAL.VerifyOtp(phoneNumber, otp);

            return record;
        }

        [HttpPost("send")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<OtpResult> SendOtp(string phoneNumber, string templateName = "OTP1")
        {
            var url = $"https://2factor.in/API/V1/{_apiKey}/SMS/{phoneNumber}/AUTOGEN2/{templateName}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OtpResult>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var otpEntity = new OtpEntity
            {
                PhoneNumber = phoneNumber,
                OtpCode = result.OTP,
            };
            _ = _otpBAL.SaveOtp(otpEntity);

            return result ?? throw new Exception("Failed to deserialize OTP response.");
        }
    }
}
