using DecorPlastsAPI.Services;
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
        private readonly JwtService _jwtService;
        public OtpController(OtpService otpService, OtpBAL otpBAL, JwtService jwtService)
        {
            _otpService = otpService;
            _otpBAL = otpBAL;
            _jwtService = jwtService;
        }

        [HttpPost("GenerateOtp")]
        public async Task<IActionResult> GenerateOtp([FromBody] OtpRequest request)
        {
            try
            {
                bool IsMobileExists = _otpBAL.CheckMobileExists(request.PhoneNumber);
                if (!IsMobileExists)
                    return BadRequest(new { Message = "Mobile number not found" });

                string otp = await _otpService.GenerateOtp(request.PhoneNumber);
                _otpService.SendOtp(request.PhoneNumber, otp);

                return Ok(new { Message = "OTP Sent Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending OTP", Details = ex.Message });
            }
        }

        [HttpPost("verifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var record = await _otpService.VerifyOtp(request.PhoneNumber, request.Otp);
                if (!record.Status)
                    return BadRequest(new { record.Message });

                var token = _jwtService.GenerateToken(request.PhoneNumber);

                return Ok(new IsverifyOtp
                {
                    Status = record.Status,
                    Message = record.Message,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while verifying OTP", Details = ex.Message });
            }
        }
    }
}
