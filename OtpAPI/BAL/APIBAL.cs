
using Dapper;
using DecorPlastsAPI.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using OtpAPI.Models;
using System;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
            DynamicParameters param = new DynamicParameters();
            param.Add("@PhoneNumber", PhoneNumber);

            var result = _DB.QueryFirstOrDefault<int>("USP_CheckMobileExists", param, commandType: CommandType.StoredProcedure);
            return result > 0;
        }
        public bool SaveOtp(OtpEntity otpEntity)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_PhoneNumber", otpEntity.PhoneNumber);
            param.Add("@P_OtpCode", otpEntity.OtpCode);
            param.Add("@P_ExpiryTime", DateTime.Now.AddMinutes(60));
            var result = _DB.QueryFirstOrDefault<int>("USP_InsertUpdateOtp", param, commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public IsverifyOtp VerifyOtp(string phoneNumber, string otp)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_PhoneNumber", phoneNumber);
            param.Add("@P_OtpCode", otp);

            var result = _DB.Query<IsverifyOtp>("USP_VerifyOtp", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            return result ?? new IsverifyOtp { Status = false, Message = "Something went wrong" };
        }
        //public string GetToken(string PhoneNumber)
        //{
        //    DynamicParameters param = new DynamicParameters();
        //    param.Add("@P_PhoneNumber", PhoneNumber);
        //    var result = _DB.Query<IsverifyOtp>("USP_GetToken", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
        //    return result ?? new IsverifyOtp { Status = false, Message = "Invalid token" };
        //}
        public bool SaveToken(string token, string mobileno)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_PhoneNumber", mobileno);
            param.Add("@P_TokenValue", token);
            return _DB.ExecuteSP("USP_SaveToken", param) > 0;
        }
        public bool Verifytoken(string userid, string token)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(userid));
            param.Add("@p_Token", token);

            var result = _DB.Query<int>("USP_VerifyToken", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result == 1;
        }
        public List<UserData> GetUserData(int userid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@u_Id", userid);
            var result = _DB.Query<UserData>("USP_GetUserData", param, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public SpResult InsertUpdateUser(InsertUpdateUser insertUpdateUser)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_Id", Convert.ToInt32(insertUpdateUser.Id));
            param.Add("@p_OwnerName", insertUpdateUser.OwnerName);
            param.Add("@p_ShopName", insertUpdateUser.ShopName);
            param.Add("@p_MobileNo", insertUpdateUser.MobileNo);
            param.Add("@p_Address", insertUpdateUser.Address);
            param.Add("@p_Role", insertUpdateUser.Role);
            param.Add("@p_IsActive", Convert.ToInt32(insertUpdateUser.IsActive));
            var result = _DB.Query<SpResult>("USP_InsertUpdateUser", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteUser(int Id,int userId) {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_Id", Id);
            param.Add("@p_UserId", userId);
            var result = _DB.Query<SpResult>("USP_DeleteUser", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public AdminDashboard GetAdminDashboardData(int userid)
        {
            AdminDashboard dashboardData = new AdminDashboard();

            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", userid);

            var result = _DB.QueryFirstOrDefault<AdminDashboard>("USP_GetDashboardCounts", param);
            return result;
        }
        public List<GetCategory> GetAllCategoryByID(int CategoryId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId);

            return _DB.Query<GetCategory>("USP_GetAllCategoryByID", param, commandType: CommandType.StoredProcedure).ToList();
        }
        public List<GetCodeByCategory> GetCodeByID(int CodeId, int CategoryId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CodeId", CodeId);
            param.Add("@p_CategoryId", CategoryId);
            var rawList = _DB.Query<GetCodeRaw>("USP_GetCodesById", param, commandType: CommandType.StoredProcedure).ToList();
            if (!rawList.Any()) return new List<GetCodeByCategory>();


            return rawList
                .GroupBy(c => new { c.CategoryId, c.CategoryName })   // group by category
                .Select(catGroup => new GetCodeByCategory
                {
                    CategoryId = catGroup.Key.CategoryId,
                    CategoryName = catGroup.Key.CategoryName,
                    Codes = catGroup
                        .GroupBy(c => c.CodeName)                      // group by CodeName, NOT CodeId
                        .Select(codeGroup => new GetCode
                        {
                            CodeId = codeGroup.Min(c => c.CodeId),      // pick a representative row id
                            CodeName = codeGroup.Key,
                            Status = codeGroup.First().Status,
                            Size = codeGroup.First().Size,
                            Quantity = codeGroup.First().Quantity,
                            Weight = codeGroup.First().Weight,
                        }).ToList()
                }).ToList();
        }

        public bool UpdateCategory(UpdateCategory UpdateCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CategoryId", Convert.ToInt32(UpdateCategory.CategoryId));
            param.Add("@p_CategoryName", UpdateCategory.CategoryName);
            param.Add("@p_IsActive", Convert.ToInt32(UpdateCategory.Status));
            var result = _DB.Query<int>("USP_UpdateCategory", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result == 1;
        }
        public bool UpdateCode(UpdateCodes UpdateCodes)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CodeId", Convert.ToInt32(UpdateCodes.CodeId));
            param.Add("@p_CodeName", UpdateCodes.CodeName);
            param.Add("@p_Size", UpdateCodes.Size);
            param.Add("@p_CategoryId", Convert.ToInt32(UpdateCodes.CategoryId));
            param.Add("@p_IsActive", Convert.ToInt32(UpdateCodes.Status));
            param.Add("@p_Weight", UpdateCodes.Weight);
            var result = _DB.Query<int>("USP_UpdateCode", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result == 1;
        }
        public bool AddCategory(AddCategory AddCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(AddCategory.userid));
            param.Add("@C_CategoryName", AddCategory.Category);

            var result = _DB.Query<int>("USP_AddCategory", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result == 1;
        }
        public SpResult AddCode(AddCode AddCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(AddCode.userid));
            param.Add("@p_CodeName", AddCode.CodeName);
            param.Add("@p_Size", AddCode.Size);
            param.Add("@p_CategoryId", Convert.ToInt32(AddCode.CategoryId));
            param.Add("@p_Weight", Convert.ToDouble(AddCode.Weight));
            SpResult result = _DB.Query<SpResult>("USP_AddCode", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCode(Delete DeleteCode)//need to create storeoprocedure 
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_Id", Convert.ToInt32(DeleteCode.Id));
            var result = _DB.Query<SpResult>("USP_DeleteCode", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCategory(Delete DeleteCategory)//need to create storeoprocedure 
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_Id", Convert.ToInt32(DeleteCategory.Id));
            var result = _DB.Query<SpResult>("USP_DeleteCategory", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult AddUpdateStock(AddUpdateStock addStock)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CategoryId", Convert.ToInt32(addStock.CategoryId));
            param.Add("@p_CodeId", Convert.ToInt32(addStock.CodeId));
            param.Add("@p_Quantity", Convert.ToInt32(addStock.Quantity));
            var result = _DB.Query<SpResult>("USP_AddUpdateStock", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public List<GetStock> GetStock(string id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_Id", Convert.ToInt32(id));
            var stockList = _DB.Query<GetStock>("USP_GetStock", param, commandType: CommandType.StoredProcedure).ToList();
            return stockList;
        }
        public List<GetOrderList> GetOrder(getdata getdata)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@u_UserId", Convert.ToInt32(getdata.userid));

            // Query flat rows from SP
            var rows = _DB.Query<dynamic>("USP_GetOrderList", param, commandType: CommandType.StoredProcedure).ToList();

            if (rows == null || !rows.Any())
                return null;

            var dict = new Dictionary<long, GetOrderList>();
            foreach (var row in rows)
            {
                long orderId = (long)row.OrderId;

                if (!dict.ContainsKey(orderId))
                {
                    dict[orderId] = new GetOrderList
                    {

                        userid = row.UserId.ToString(),
                        username = row.UserName,
                        MobileNo = row.MobileNo.ToString(),
                        OrderId = row.OrderId,
                        Status = row.Status,
                        DateTime = row.CreatedAt.ToString(),
                        items = new List<OrderItem>()
                    };  
                }
                dict[orderId].items.Add(new OrderItem
                {
                    // Map each row to OrderItem

                    CategoryId = row.OrderCategoryId.ToString(),
                    CategoryName = row.CategoryName,
                    CodeId = row.OrderCodeId.ToString(),
                    CodeName = row.CodeName,
                    Size = row.Size,
                    Quantity = row.Quantity.ToString(),
                    Weight = row.Weight,     
                    VehicleNo = row.VehicleNo,
                    InvoiceNo = row.InvoiceNo

                });
            }
            return dict.Values.ToList();
        }
        public List<GetFutureOrderList> GetFutureOrder(getdata getdata)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@u_UserId", Convert.ToInt32(getdata.userid));

            // Query flat rows from SP
            var rows = _DB.Query<dynamic>("USP_GetFutureOrderList", param, commandType: CommandType.StoredProcedure).ToList();

            if (rows == null || !rows.Any())
                return null;

            var dict = new Dictionary<long, GetFutureOrderList>();
            foreach (var row in rows)
            {
                long orderId = (long)row.OrderId;

                if (!dict.ContainsKey(orderId))
                {
                    dict[orderId] = new GetFutureOrderList
                    {

                        userid = row.UserId.ToString(),
                        username = row.UserName,
                        MobileNo = row.MobileNo.ToString(),
                        OrderId = row.OrderId,
                        Status = row.Status,
                        DateTime = row.CreatedAt.ToString(),
                        items = new List<FutureOrderItem>()
                    };
                }
                dict[orderId].items.Add(new FutureOrderItem
                {
                    // Map each row to FutureOrderItem

                    CategoryId = row.OrderCategoryId.ToString(),
                    CategoryName = row.CategoryName,
                    CodeId = row.OrderCodeId.ToString(),
                    CodeName = row.CodeName,
                    Size = row.Size,
                    RemainQuantity = row.RemainQuantity.ToString(),
                    TotalQuantity = row.TotalQuantity.ToString(),
                    Weight = row.Weight,
                    VehicleNo = row.VehicleNo,
                    InvoiceNo = row.InvoiceNo
                });
            }
            return dict.Values.ToList();
        }
        public List<SpResult> InsertOrder(InsertUpdateOrder insertUpdateOrder)
        {
            var results = new List<SpResult>();

            foreach (var item in insertUpdateOrder.items)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@p_UserId", Convert.ToInt32(insertUpdateOrder.userid));
                param.Add("@p_OrderId", insertUpdateOrder.OrderId);
                param.Add("@p_OrderCategoryId", Convert.ToInt32(item.CategoryId));
                param.Add("@p_OrderCodeId", Convert.ToInt32(item.CodeId));
                param.Add("@p_Quantity", Convert.ToInt32(item.Quantity));
                param.Add("@p_Weight", item.Weight);
                param.Add("@p_VehicleNo", item.VehicleNo);
                param.Add("@p_InvoiceNo", item.InvoiceNo);
                param.Add("@p_Status", insertUpdateOrder.Status);

                var result = _DB.QueryFirstOrDefault<SpResult>("USP_InsertOrder", param, commandType: CommandType.StoredProcedure);
                results.Add(result);
            }
            return results;
        }
        public SpResult UpdateOrder(InsertUpdateOrder insertUpdateOrder)
        {
            SpResult result = new SpResult();
            foreach (var item in insertUpdateOrder.items)
            {
                var param = new DynamicParameters();
                param.Add("p_userid", Convert.ToInt32(insertUpdateOrder.userid));
                param.Add("p_orderid", insertUpdateOrder.OrderId);
                param.Add("p_ordercategoryid", Convert.ToInt32(item.CategoryId));
                param.Add("p_ordercodeid", Convert.ToInt32(item.CodeId));
                param.Add("p_quantity", Convert.ToInt32(item.Quantity));
                param.Add("p_weight", item.Weight);
                param.Add("p_vehicleNo", item.VehicleNo);
                param.Add("p_invoiceNo", item.InvoiceNo);
                param.Add("p_status", insertUpdateOrder.Status);

                result = _DB.QueryFirstOrDefault<SpResult>("usp_UpdateOrder",param,commandType: CommandType.StoredProcedure);
            }
            return result;
        }
        public bool CheckStock(int categoryId, int codeId,  int quantity)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_OrderCategoryId", categoryId);
            param.Add("@p_OrderCodeId", codeId);
            param.Add("@p_Quantity", quantity);

            var result = _DB.QueryFirstOrDefault<StockCheckResult>("USP_CheckStock", param, commandType: CommandType.StoredProcedure);
            return result?.IsAvailable ?? false;
        }
        public SpResult UserLogout(int userid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserId", userid);
            var result = _DB.Query<SpResult>("USP_Logout", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult InsertUpdateCart(InsertUpdateCart insertUpdateCart)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_id", Convert.ToInt32(insertUpdateCart.CartId));
            param.Add("@p_UserId", Convert.ToInt32(insertUpdateCart.userid));
            param.Add("@p_CartCategoryId", Convert.ToInt32(insertUpdateCart.CategoryId));
            param.Add("@p_CartCodeId", Convert.ToInt32(insertUpdateCart.CodeId));
            param.Add("@p_quantity", Convert.ToInt32(insertUpdateCart.Quantity));
            var result = _DB.Query<SpResult>("USP_InsertUpdateCart", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public GetCart GetCart(string id,string userid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_id", Convert.ToInt32(id));
            param.Add("@p_UserId", Convert.ToInt32(userid));
            var result = _DB.Query<GetCart>("USP_GetCart", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
    }
}
