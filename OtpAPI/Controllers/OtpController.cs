using DecorPlastsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.MicrosoftExtensions;
using OtpAPI.BAL;
using OtpAPI.Models;
using OtpAPI.Services;
using Twilio.TwiML.Messaging;

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
        public IActionResult GenerateOtp([FromBody] OtpRequest request)
        {
            try
            {
                bool IsMobileExists = _otpBAL.CheckMobileExists(request.PhoneNumber);
                if (!IsMobileExists)
                    return BadRequest(new { Message = "Mobile number not found" });

                string otp =  _otpService.GenerateOtp(request.PhoneNumber);
                var mobileno = "+91"+request.PhoneNumber;
                _otpService.SendOtp(mobileno, otp);

                return Ok(new { Message = "OTP Sent Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending OTP", Details = ex.Message });
            }
        }

        [HttpPost("verifyOtp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                IsverifyOtp IsverifyOtp = _otpService.VerifyOtp(request.PhoneNumber, request.Otp);
                if (!IsverifyOtp.Status)
                    return BadRequest(new { IsverifyOtp.Message });

                 IsverifyOtp.Token = _jwtService.GenerateToken(request.PhoneNumber);
                _otpBAL.SaveToken(IsverifyOtp.Token, request.PhoneNumber);

                return Ok(IsverifyOtp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while verifying OTP", Details = ex.Message });
            }
        }
        [HttpPost("GetAdminDashbord")]
        public IActionResult GetAdminDashbordData([FromBody] getdata getdata)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    AdminDasshboard AdminDasshboard =  _otpBAL.GetAdminDashboardData(Convert.ToInt32(getdata.userid));
                    return Ok(AdminDasshboard);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Admin Dashbord Data", Details = ex.Message });
            }
        }
    }
}
