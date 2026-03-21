
using Dapper;
using DecorPlastsAPI.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using OtpAPI.Models;
using System;
using System.Data;
using Twilio.Jwt.AccessToken;
namespace OtpAPI.BAL
{
    public class APIBAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataRepository _DB;

        public APIBAL(IConfiguration configuration, IDataRepository DB)
        {
            _configuration = configuration;
            _DB = DB;
        }
        public bool CheckMobileExists(string PhoneNumber)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("USP_CheckMobileExists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                    con.Open();
                    int status = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();

                    return status == 1;
                }
            }
        }
        public bool SaveOtp(OtpEntity otpEntity)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@PhoneNumber", otpEntity.PhoneNumber);
            param.Add("@OtpCode", otpEntity.OtpCode);
            param.Add("@ExpiryTime", DateTime.Now.AddMinutes(5));

            return _DB.ExecuteSP("USP_SaveOtp", param) > 0;
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_PhoneNumber", phoneNumber);
            param.Add("@O_OtpCode", otp);

            var result = _DB.Query<IsverifyOtp>("USP_VerifyOtp", param).FirstOrDefault();

            return result ?? new IsverifyOtp { Status = false, Message = "Something went wrong" };
        }
        public bool SaveToken(string token, string mobileno)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@PhoneNumber", mobileno);
            param.Add("@TokenValue", token);
            return _DB.ExecuteSP("USP_SaveToken", param) > 0;
        }
        public bool Verifytoken(string userid, string token)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(userid));
            param.Add("@p_Token", token);

            var result = _DB.Query<int>("USP_VerifyToken", param).FirstOrDefault();
            return result == 1;
        }
        public AdminDashboard GetAdminDashboardData(int userid)
        {
            AdminDashboard dashboardData = new AdminDashboard();

            DynamicParameters param = new DynamicParameters();
            param.Add("@Userid", userid);

            var result = _DB.QueryFirstOrDefault<AdminDashboard>("USP_GetDashboardCounts", param);

            if (result != null)
            {
                dashboardData = result;
            }
            return dashboardData;
        }
        public List<GetCategory> GetAllCategoryByID(int CategoryId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId);

            return _DB.Query<GetCategory>("USP_GetAllCategoryByID", param).ToList();
        }
        public List<GetCodeByCategory> GetCodeByID(int CodeId, int CategoryId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CodeId", CodeId);
            param.Add("@CategoryId", CategoryId);
            var rawList = _DB.Query<GetCodeRaw>("USP_GetCodesById", param).ToList();
            if (!rawList.Any()) return new List<GetCodeByCategory>();

            return rawList
                .GroupBy(c => new { c.CategoryId, c.CategoryName })  // Group by category
                .Select(group => new GetCodeByCategory
                {
                    CategoryId = group.Key.CategoryId,
                    CategoryName = group.Key.CategoryName,
                    Codes = group.Select(c =>
                    {
                        var sizeIdArray = c.SizeId?.Split(',')
                                             .Select(s => int.Parse(s.Trim()))
                                             .ToList() ?? new List<int>();

                        var sizeNameArray = c.Sizes?.Split(',')
                                             .Select(s => s.Trim())
                                             .ToList() ?? new List<string>();

                        var quantityArray = c.Quantity?.Split(',')
                                             .Select(s => int.Parse(s.Trim()))
                                             .ToList() ?? new List<int>();

                        return new GetCode
                        {
                            CodeId = c.CodeId,
                            CodeName = c.CodeName,
                            Status = c.Status,
                            Sizes = sizeIdArray
                                .Select((id, index) => new SizeItem
                                {
                                    SizeId = id,
                                    Size = sizeNameArray.ElementAtOrDefault(index) ?? "",
                                    Quantity = quantityArray.ElementAtOrDefault(index)
                                }).ToList()
                        };
                    }).ToList()
                }).ToList();
        }
        public List<GetSize> GetSizeByID(int SizeId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SizeId", SizeId);
            return _DB.Query<GetSize>("UPS_GetSizeByID", param).ToList();
        }

        public bool UpdateCategory(UpdateCategory UpdateCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CategoryId", Convert.ToInt32(UpdateCategory.CategoryId));
            param.Add("@p_CategoryName", UpdateCategory.CategoryName);
            param.Add("@p_IsActive", Convert.ToInt32(UpdateCategory.Status));
            var result = _DB.Query<int>("USP_UpdateCategory", param).FirstOrDefault();
            return result == 1;
        }
        public bool UpdateCode(UpdateCodes UpdateCodes)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CodeId", Convert.ToInt32(UpdateCodes.CodeId));
            param.Add("@p_CodeName", UpdateCodes.CodeName);
            param.Add("@p_SizeId", Convert.ToInt32(UpdateCodes.SizeId));
            param.Add("@p_CategoryId", Convert.ToInt32(UpdateCodes.CategoryId));
            param.Add("@p_IsActive", Convert.ToInt32(UpdateCodes.Status));
            var result = _DB.Query<int>("USP_UpdateCode", param).FirstOrDefault();
            return result == 1;
        }
        public SpResult UpdateSize(UpdateSize UpdateSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_UserId", Convert.ToInt32(UpdateSize.userid));
            param.Add("@p_Id", Convert.ToInt32(UpdateSize.SizeId));
            param.Add("@p_Size", UpdateSize.Size);
            param.Add("@p_Status", Convert.ToInt32(UpdateSize.Status));
            var result = _DB.Query<SpResult>("USP_UpdateSize", param).FirstOrDefault();
            return result;
        }
        public bool AddCategory(AddCategory AddCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserId", Convert.ToInt32(AddCategory.userid));
            param.Add("@C_CategoryName", AddCategory.Category);

            var result = _DB.Query<int>("USP_AddCategory", param).FirstOrDefault();
            return result == 1;
        }
        public SpResult AddCode(AddCode AddCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(AddCode.userid));
            param.Add("@p_CodeName", AddCode.CodeName);
            param.Add("@p_SizeId", AddCode.SizeId);
            param.Add("@p_CategoryId", Convert.ToInt32(AddCode.CategoryId));
            var result = _DB.Query<SpResult>("USP_AddCode", param).FirstOrDefault();
            return result;
        }
        public SpResult AddSize(AddSize AddSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(AddSize.userid));
            param.Add("@p_Size", AddSize.Size);
            var result = _DB.Query<SpResult>("USP_AddSize", param).FirstOrDefault();
            return result;
        }
        public SpResult DeleteSize(Delete DeleteSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SizeId", Convert.ToInt32(DeleteSize.Id));
            param.Add("@I_IsDelete", DeleteSize.IsDelete);
            var result = _DB.Query<SpResult>("USP_DeleteSize", param).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCode(Delete DeleteCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CodeId", Convert.ToInt32(DeleteCode.Id));
            param.Add("@I_IsDelete", DeleteCode.IsDelete);
            var result = _DB.Query<SpResult>("USP_DeleteSize", param).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCategory(Delete DeleteCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@I_CategoryId", Convert.ToInt32(DeleteCategory.Id));
            param.Add("@I_IsDelete", DeleteCategory.IsDelete);
            var result = _DB.Query<SpResult>("USP_DeleteCategory", param).FirstOrDefault();
            return result;
        }
        public SpResult AddStock(AddStock addStock)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CategoryId", Convert.ToInt32(addStock.CategoryId));
            param.Add("@p_CodeId", Convert.ToInt32(addStock.CodeId));
            param.Add("@p_SizeId", Convert.ToInt32(addStock.SizeId));
            param.Add("@p_Quantity", Convert.ToInt32(addStock.Quantity));
            var result = _DB.Query<SpResult>("USP_AddUpdateStock", param).FirstOrDefault();
            return result;
        }
        public SpResult InsertUpdateOrder(InsertUpdateOrder insertUpdateOrder)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_UserId", Convert.ToInt32(insertUpdateOrder.userid));
            parameters.Add("@p_Id", Convert.ToInt32(insertUpdateOrder.OrderId));
            parameters.Add("@p_OrderCategoryId", Convert.ToInt32(insertUpdateOrder.CategoryId));
            parameters.Add("@p_OrderCodeId", Convert.ToInt32(insertUpdateOrder.CodeId));
            parameters.Add("@p_OrderSizeId", Convert.ToInt32(insertUpdateOrder.SizeId));
            parameters.Add("@p_Quantity", Convert.ToInt32(insertUpdateOrder.Quantity));
            parameters.Add("@p_Status", insertUpdateOrder.Status);
            var result = _DB.Query<SpResult>("USP_InsertUpdateOrder", parameters).FirstOrDefault();
            return result;
        }
        public List<OrderDetails> GetOrderDetails(int UserId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserId", UserId);
            return _DB.Query<OrderDetails>("USP_GetOrderById", param).ToList();
        }
    }
}
