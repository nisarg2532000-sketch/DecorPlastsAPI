using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.MicrosoftExtensions;
using OtpAPI.BAL;
using OtpAPI.Models;
using OtpAPI.Services;

namespace OtpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : Controller
    {
        private readonly OtpService _otpService;
        private readonly OtpBAL _otpBAL;
        public OtpController(OtpService otpService, OtpBAL otpBAL)
        {
            _otpService = otpService;
            _otpBAL = otpBAL;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            bool IsMobileExists = _otpBAL.CheckMobileExists(request.PhoneNumber);
            if (!IsMobileExists)
                return BadRequest(new { Message = "Mobile number not found" });

            string otp = await _otpService.GenerateOtp(request.PhoneNumber);
            _otpService.SendOtp(request.PhoneNumber, otp);

            return Ok(new { Message = "OTP Sent Successfully" });
        }

        [HttpPost("verify")]
        public async Task<IsverifyOtp> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            IsverifyOtp record = await _otpService.VerifyOtp(request.PhoneNumber, request.Otp);

            return new IsverifyOtp
            {
                Status = record.Status,
                Message = record.Message,
            };
        }
    }
}
