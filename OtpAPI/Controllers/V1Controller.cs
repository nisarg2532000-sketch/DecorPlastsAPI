using DecorPlastsAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.MicrosoftExtensions;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using OtpAPI.BAL;
using OtpAPI.Models;
using OtpAPI.Services;

namespace OtpAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class V1Controller : Controller
    {
        private readonly OtpService _otpService;
        private readonly APIBAL _otpBAL;
        private readonly JwtService _jwtService;
        public V1Controller(OtpService otpService, APIBAL otpBAL, JwtService jwtService)
        {
            _otpService = otpService;
            _otpBAL = otpBAL;
            _jwtService = jwtService;
        }
        [HttpPost("GetAppVersion")]
        public IActionResult GetAppVersion()
        {

            var response = new AppVersionResponse
            {
                LatestVersion = "1.0.0",

                ForceUpdate = true,

                Message = "New order and stock management features added."
            };

            return Ok(response);
        }
        [HttpPost("GenerateOtp")]
        public async Task<IActionResult> GenerateOtp([FromBody] APIRequest request)
        {
            try
            {
                bool IsMobileExists = _otpBAL.CheckMobileExists(request.PhoneNumber);
                if (!IsMobileExists)
                    return BadRequest(new { Message = "Mobile number not found" });

                var mobileno = "+91" + request.PhoneNumber;
                await _otpService.SendOtp(mobileno);

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

                var mobileno = "+91" + request.PhoneNumber;
                IsverifyOtp IsverifyOtp = _otpService.VerifyOtp(request.PhoneNumber, request.Otp);
                if (!IsverifyOtp.Status)
                    return BadRequest(new { IsverifyOtp.Message });

                //IsverifyOtp.Token = _otpBAL.GetToken(request.PhoneNumber);
                if (IsverifyOtp.Token != null)
                { 
                    return Ok(IsverifyOtp);
                }
                IsverifyOtp.Token = _jwtService.GenerateToken(request.PhoneNumber);
                _otpBAL.SaveToken(IsverifyOtp.Token, request.PhoneNumber);

                return Ok(IsverifyOtp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while verifying OTP", Details = ex.Message });
            }
        }
        [HttpPost("GetUserData")]
        public IActionResult GetUserData([FromBody] getdata getdata, string id)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var userData = _otpBAL.GetUserData(Convert.ToInt32(id));
                    return Ok(userData);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get User Data", Details = ex.Message });
            }
        }
        [HttpPost("InsertUpdateUser")]
        public IActionResult InsertUpdateUser([FromBody] InsertUpdateUser insertUpdateUser)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(insertUpdateUser.userid, insertUpdateUser.token);
                if (issucess)
                {
                    SpResult userData = _otpBAL.InsertUpdateUser(insertUpdateUser);
                    return Ok(userData);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get User Data", Details = ex.Message });
            }
        }
        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser([FromBody] getdata getdata, string id)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var status = _otpBAL.DeleteUser(Convert.ToInt32(id), Convert.ToInt32(getdata.userid));
                    return Ok(status);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Delete User", Details = ex.Message });
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
                    AdminDashboard AdminDasshboard = _otpBAL.GetAdminDashboardData(Convert.ToInt32(getdata.userid));
                    return Ok(AdminDasshboard);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Admin Dashbord Data", Details = ex.Message });
            }
        }
        [HttpPost("GetCategoryById")]
        public IActionResult GetCategory([FromBody] getdata getdata, string CategoryId)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var category = _otpBAL.GetAllCategoryByID(Convert.ToInt32(CategoryId));
                    return Ok(category);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Category", Details = ex.Message });
            }
        }
        [HttpPost("GetCodeById")]
        public IActionResult GetCode([FromBody] getdata getdata, string CodeId, string CategoryId)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var codes = _otpBAL.GetCodeByID(Convert.ToInt32(CodeId), Convert.ToInt32(CategoryId));
                    return Ok(codes);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Category", Details = ex.Message });
            }
        }
        [HttpPost("UpdateCategory")]
        public IActionResult UpdateCategory([FromBody] UpdateCategory UpdateCategory)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(UpdateCategory.userid, UpdateCategory.token);
                if (issucess)
                {
                    var category = _otpBAL.UpdateCategory(UpdateCategory);
                    return Ok(category);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Update Category", Details = ex.Message });
            }
        }
        [HttpPost("UpdateCode")]
        public IActionResult UpdateCode([FromBody] UpdateCodes UpdateCodes)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(UpdateCodes.userid, UpdateCodes.token);
                if (issucess)
                {
                    var Codes = _otpBAL.UpdateCode(UpdateCodes);
                    return Ok(Codes);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Update Codes", Details = ex.Message });
            }
        }
       
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] AddCategory AddCategory)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(AddCategory.userid, AddCategory.token);
                if (issucess)
                {
                    var category = _otpBAL.AddCategory(AddCategory);
                    return Ok(category);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Add Category", Details = ex.Message });
            }
        }
        [HttpPost("AddCode")]
        public IActionResult AddCode([FromBody] AddCode AddCode)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(AddCode.userid, AddCode.token);
                if (issucess)
                {
                    SpResult Codes = _otpBAL.AddCode(AddCode);
                    return Ok(Codes);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Add Codes", Details = ex.Message });
            }
        }
        [HttpPost("DeleteCode")]
        public IActionResult DeleteCode([FromBody] Delete DeleteCode)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(DeleteCode.userid, DeleteCode.token);
                if (issucess)
                {
                    var deletecode = _otpBAL.DeleteCode(DeleteCode);
                    return Ok(deletecode);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Delete Code", Details = ex.Message });
            }
        }
        [HttpPost("DeleteCategory")]
        public IActionResult DeleteCrategory([FromBody] Delete DeleteCategory)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(DeleteCategory.userid, DeleteCategory.token);
                if (issucess)
                {
                    var deletecategory = _otpBAL.DeleteCategory(DeleteCategory);
                    return Ok(DeleteCategory);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Delete Category", Details = ex.Message });
            }
        }
        [HttpPost("AddUpdateStock")]
        public IActionResult AddUpdateStock([FromBody] AddUpdateStock AddStock)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(AddStock.userid, AddStock.token);
                if (issucess)
                {
                    SpResult addstock = _otpBAL.AddUpdateStock(AddStock);
                    return Ok(addstock);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Add Update Stock", Details = ex.Message });
            }
        }
        [HttpPost("GetStock")]
        public IActionResult GetStock([FromBody] getdata getdata,string id)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var stocklist = _otpBAL.GetStock(id);
                    return Ok(stocklist);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Add Update Stock", Details = ex.Message });
            }
        }
        [HttpPost("GetOrder")]
        public IActionResult GetOrder([FromBody] getdata getdata, string status)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var getOrderList = _otpBAL.GetOrder(getdata, status);
                    return Ok(getOrderList);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Order", Details = ex.Message });
            }
        }
        [HttpPost("GetOrderByUserId")]
        public IActionResult GetOrderByUserId([FromBody] getdata getdata, string status)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var getOrderList = _otpBAL.GetOrderByUserId(getdata, status);
                    return Ok(getOrderList);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Order", Details = ex.Message });
            }
        }
        [HttpPost("GetFutureOrder")]
        public IActionResult GetFutureOrder([FromBody] getdata getdata, string status)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var getFutureOrderList = _otpBAL.GetFutureOrder(getdata, status);
                    return Ok(getFutureOrderList);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Future Order", Details = ex.Message });
            }
        }
        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder([FromBody] InsertOrder insertUpdateOrder)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(insertUpdateOrder.userid, insertUpdateOrder.token);
                if (issucess)
                {   
                    var results = _otpBAL.InsertOrder(insertUpdateOrder);
                    return Ok(results);
                    
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Insert Order", Details = ex.Message });
            }
        }
        [HttpPost("UpdateOrder")]
        public IActionResult UpdateOrder([FromBody] UpdateOrder updateOrder)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(updateOrder.userid, updateOrder.token);
                if (issucess)
                {
                    var results = _otpBAL.UpdateOrder(updateOrder);
                    return Ok(results);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Update Order", Details = ex.Message });
            }
        }
        [HttpPost("GetTotalWeightByUser")]
        public IActionResult GetTotalWeightByUser([FromBody] getdata getdata, string Id)
        {
            
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var totalWeight = _otpBAL.GetTotalWeightByUser(Convert.ToInt32(Id));
                    return Ok(totalWeight);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Get Total Weight By User", Details = ex.Message });
            }
            
        }
        [HttpPost("InsertUpdateCart")]
        public IActionResult InsertUpdateCart([FromBody] InsertUpdateCart insertUpdateCart)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(insertUpdateCart.userid, insertUpdateCart.token);
                if (issucess)
                {
                    SpResult cart = _otpBAL.InsertUpdateCart(insertUpdateCart);
                    return Ok(cart);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while Insert Update Cart", Details = ex.Message });
            }
        }
        [HttpPost("GetCart")]
        public IActionResult GetCart([FromBody] getdata getdata , string id)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    GetCart cart = _otpBAL.GetCart(id, getdata.userid);
                    return Ok(cart);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while get Cart", Details = ex.Message });
            }
        }
        [HttpPost("UserLogout")]
        public IActionResult UserLogout([FromBody] getdata getdata)
        {
            try
            {
                bool issucess = _otpBAL.Verifytoken(getdata.userid, getdata.token);
                if (issucess)
                {
                    var status = _otpBAL.UserLogout(Convert.ToInt32(getdata.userid));
                    return Ok(status);
                }
                return BadRequest(new { Message = "Token not verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while User Logout", Details = ex.Message });
            }
        }
    }
}