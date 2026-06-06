
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
            param.Add("@O_OtpCode", otp);

            var result = _DB.Query<IsverifyOtp>("USP_VerifyOtp", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

            return result ?? new IsverifyOtp { Status = false, Message = "Something went wrong" };
        }
        public IsverifyOtp GetToken(string PhoneNumber)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_PhoneNumber", PhoneNumber);
            var result = _DB.Query<IsverifyOtp>("USP_GetToken", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result ?? new IsverifyOtp { Status = false, Message = "Invalid token" };
        }
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
            param.Add("@p_SizeId", SizeId);
            return _DB.Query<GetSize>("UPS_GetSizeByID", param, commandType: CommandType.StoredProcedure).ToList();
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
            param.Add("@p_SizeId", string.Join(",", UpdateCodes.SizeIds));
            param.Add("@p_CategoryId", Convert.ToInt32(UpdateCodes.CategoryId));
            param.Add("@p_IsActive", Convert.ToInt32(UpdateCodes.Status));
            var result = _DB.Query<int>("USP_UpdateCode", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result == 1;
        }
        public SpResult UpdateSize(UpdateSize UpdateSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_UserId", Convert.ToInt32(UpdateSize.userid));
            param.Add("@p_Id", Convert.ToInt32(UpdateSize.SizeId));
            param.Add("@p_Size", UpdateSize.Size);
            param.Add("@p_Status", Convert.ToInt32(UpdateSize.Status));
            var result = _DB.Query<SpResult>("USP_UpdateSize", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
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
            param.Add("@p_SizeId", string.Join(",", AddCode.SizeIds));
            param.Add("@p_CategoryId", Convert.ToInt32(AddCode.CategoryId));
            SpResult result = _DB.Query<SpResult>("USP_AddCode", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult AddSize(AddSize AddSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_UserId", Convert.ToInt32(AddSize.userid));
            param.Add("@p_Size", AddSize.Size);
            var result = _DB.Query<SpResult>("USP_AddSize", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteSize(Delete DeleteSize)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SizeId", Convert.ToInt32(DeleteSize.Id));
            param.Add("@I_IsDelete", DeleteSize.IsDelete);

            var result = _DB.Query<SpResult>("USP_DeleteSize", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCode(Delete DeleteCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@CodeId", Convert.ToInt32(DeleteCode.Id));
            param.Add("@I_IsDelete", DeleteCode.IsDelete);
            var result = _DB.Query<SpResult>("USP_DeleteSize", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult DeleteCategory(Delete DeleteCategory)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@I_CategoryId", Convert.ToInt32(DeleteCategory.Id));
            param.Add("@I_IsDelete", DeleteCategory.IsDelete);
            var result = _DB.Query<SpResult>("USP_DeleteCategory", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }
        public SpResult AddUpdateStock(AddUpdateStock addStock)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_CategoryId", Convert.ToInt32(addStock.CategoryId));
            param.Add("@p_CodeId", Convert.ToInt32(addStock.CodeId));
            param.Add("@p_SizeId", Convert.ToInt32(addStock.SizeId));
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
                    SizeId = row.OrderSizeId.ToString(),
                    Size = row.Size,
                    Quantity = row.Quantity.ToString()
                });
            }
            return dict.Values.ToList();
        }
        public List<GetOrderList> GetFutureOrder(getdata getdata)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@u_UserId", Convert.ToInt32(getdata.userid));

            // Query flat rows from SP
            var rows = _DB.Query<dynamic>("USP_GetFutureOrderList", param, commandType: CommandType.StoredProcedure).ToList();

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
                    SizeId = row.OrderSizeId.ToString(),
                    Size = row.Size,
                    Quantity = row.Quantity.ToString()
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
                param.Add("@p_OrderSizeId", Convert.ToInt32(item.SizeId));
                param.Add("@p_Quantity", Convert.ToInt32(item.Quantity));
                param.Add("@p_Status", insertUpdateOrder.Status);

                var result = _DB.QueryFirstOrDefault<SpResult>("USP_InsertUpdateOrder", param, commandType: CommandType.StoredProcedure);
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
                param.Add("p_ordersizeid", Convert.ToInt32(item.SizeId));
                param.Add("p_quantity", Convert.ToInt32(item.Quantity));
                param.Add("p_status", insertUpdateOrder.Status);

                result = _DB.QueryFirstOrDefault<SpResult>("usp_UpdateOrder",param,commandType: CommandType.StoredProcedure);
            }
            return result;
        }
        public bool CheckStock(int categoryId, int codeId, int sizeId, int quantity)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@p_OrderCategoryId", categoryId);
            param.Add("@p_OrderCodeId", codeId);
            param.Add("@p_OrderSizeId", sizeId);
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
            param.Add("@p_categoryid", Convert.ToInt32(insertUpdateCart.CategoryId));
            param.Add("@p_codeid", Convert.ToInt32(insertUpdateCart.CodeId));
            param.Add("@p_sizeid", Convert.ToInt32(insertUpdateCart.SizeId));
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
