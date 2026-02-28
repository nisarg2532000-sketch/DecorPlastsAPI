
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OtpAPI.Models;
using System;
using System.Data;
namespace OtpAPI.BAL
{
    public class OtpBAL
    {
        private readonly IConfiguration _configuration;

        public OtpBAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool CheckMobileExists(string PhoneNumber)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_CheckMobileExists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    return rows > 0;
                }
            }
        }
        public bool SaveOtp(OtpEntity OtpEntity)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_SaveOtp", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PhoneNumber", OtpEntity.PhoneNumber);
                    cmd.Parameters.AddWithValue("@OtpCode", OtpEntity.OtpCode);
                    cmd.Parameters.AddWithValue("@ExpiryTime", DateTime.Now.AddMinutes(5));

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    return rows > 0;
                }
            }
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            IsverifyOtp response = new IsverifyOtp();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_VerifyOtp", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@OtpCode", otp);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            response.Status = Convert.ToBoolean(reader["Status"]);
                            response.Message = reader["Message"].ToString();
                        }
                    }
                    con.Close();

                    return response;
                }
            }
        }
        public bool SaveToken(Savetoken savetoken)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_SaveToken", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PhoneNumber", savetoken.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Token", savetoken.Token);
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();
                    return rows > 0;
                }
            }
        }
    }
}
