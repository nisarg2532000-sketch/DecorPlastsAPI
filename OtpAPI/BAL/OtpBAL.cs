
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
    public class OtpBAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataRepository _DB;

        public OtpBAL(IConfiguration configuration, IDataRepository DB)
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
            var param = new DynamicParameters();
            param.Add("@PhoneNumber", otpEntity.PhoneNumber);
            param.Add("@OtpCode", otpEntity.OtpCode);
            param.Add("@ExpiryTime", DateTime.Now.AddMinutes(5));

            return _DB.ExecuteSP("USP_SaveOtp", param) > 0;
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            var param = new DynamicParameters();
            param.Add("@PhoneNumber", phoneNumber);
            param.Add("@OtpCode", otp);

            var result = _DB.Query<IsverifyOtp>("USP_VerifyOtp", param).FirstOrDefault();

            return result ?? new IsverifyOtp { Status = false, Message = "Something went wrong" };
        }
        public bool SaveToken(string token, string mobileno)
        {
            var param = new DynamicParameters();
            param.Add("@PhoneNumber", mobileno);
            param.Add("@TokenValue", token);
            return _DB.ExecuteSP("USP_SaveToken", param) > 0;
        }
        public bool Verifytoken(string userid, string token)
        {
            var param = new DynamicParameters();
            param.Add("@userid", Convert.ToInt32(userid));
            param.Add("@token", token);

            var result = _DB.Query<int>("USP_VerifyToken", param).FirstOrDefault();
            return result == 1;
        }
        public AdminDashboard GetAdminDashboardData(int userid)
        {
            AdminDashboard dashboardData = new AdminDashboard();

            var param = new DynamicParameters();
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
            var param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId);

            return _DB.Query<GetCategory>("USP_GetAllCategoryByID", param).ToList();
        }
        public List<GetCode> GetCodeByID(int CodeId)
        {
            var param = new DynamicParameters();
            param.Add("@CodeId", CodeId);
            return _DB.Query<GetCode>("USP_GetCodesById", param).ToList();
        }
        public List<GetSize> GetSizeByID(int SizeId)
        {
            var param = new DynamicParameters();
            param.Add("@SizeId", SizeId);
            return _DB.Query<GetSize>("UPS_GetSizeByID", param).ToList();
        }
    }
}
