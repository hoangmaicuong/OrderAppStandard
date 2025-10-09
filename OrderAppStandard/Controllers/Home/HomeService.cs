using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using OrderApp.Areas.Admin.Controllers.AdminProduct;
using System.Web.UI.WebControls;
using OrderApp.Controllers.ExternalServices;
using System.IO;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace OrderApp.Controllers.Home
{
    public class HomeService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        private static readonly MemoryCache _cache = MemoryCache.Default;
        public DataSet GetAll(string companySlug)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "HomeModuleGetAll";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@companySlug",
                            SqlDbType = SqlDbType.VarChar,
                            Value = companySlug
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "products";
                            dataSet.Tables[1].TableName = "categorys";
                            return dataSet;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }
        public Support.ResponsesAPI CreateOrder(string companySlug, HomeDto.CreateOrderDto dto, string serviceAccountPath)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = new Order();

            #endregion
            var table = db.Table.FirstOrDefault(x => x.TableId == dto.Order.TableId && x.TableToken == dto.Order.TableToken);
            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (dto.Order.TableId < 1 || table == null)
            {
                result.success = false;
                result.messageForUser = "Chưa có bàn.";
                return result;
            }
            var company = db.Company.FirstOrDefault(x => x.Slug == companySlug);
            if (company == null)
            {
                result.success = false;
                result.messageForUser = "Chưa có quán.";
                return result;
            }
            order.TableId = dto.Order.TableId;
            order.CompanyId = company.CompanyId;
            order.OrderDate = DateTime.UtcNow.AddHours(7);
            order.IsFinish = false;

            foreach (var item in dto.OrderDetails)
            {
                order.OrderDetail.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderDetailQuantity = item.ShoppingCartQuantity,
                    OrderDetailNote = item.ShoppingCartNote
                });
            }
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Order.Add(order);
                    db.SaveChanges();
                    transaction.Commit();

                    var firebase = new FirebaseHelper(serviceAccountPath);
                    var firebaseTokens = company.UserExtension.Where(x => !string.IsNullOrEmpty(x.FirebaseToken)).Select(x => x.FirebaseToken).Distinct().ToList();
                    foreach (var token in firebaseTokens)
                    {
                        Task.Run(() => firebase.SendNotificationToTokenAsync(
                            token,
                            "Đơn hàng mới",
                            $"#{order.OrderId} - {table.TableName} đặt",
                            null,
                            new Dictionary<string, string>
                            {
                                { "orderId", order.OrderId.ToString() },
                                { "type", "new-order" }
                            }
                        ));
                    }
                    result = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = order.OrderId
                        }
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result = new Support.ResponsesAPI
                    {
                        success = false,
                        messageForUser = Support.ResponsesAPI.MessageAPI.messageException,
                        messageForDev = ex.Message
                    };
                }
            }
            #endregion

            //* Kết quả hàm *
            return result;
        }
        public DataSet GetOrderOfTable(int tableId, Guid tableToken)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "HomeModuleGetOrderOfTable";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@tableId",
                            SqlDbType = SqlDbType.Int,
                            Value = tableId
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@tableToken",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            Value = tableToken
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "orders";
                            dataSet.Tables[1].TableName = "orderDetails";
                            return dataSet;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }
        public DataSet GetTable(int tableId, Guid tableToken)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "HomeModuleGetTable";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@tableId",
                            SqlDbType = SqlDbType.Int,
                            Value = tableId
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@tableToken",
                            SqlDbType = SqlDbType.UniqueIdentifier,
                            Value = tableToken
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "tables";
                            return dataSet;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }
        public Support.ResponsesAPI CallStaff(int tableId, Guid tableToken, string serviceAccountPath)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            #endregion

            #region Kiểm tra điều kiện thực thi function
            var table = db.Table.FirstOrDefault(x => x.TableId == tableId && x.TableToken == tableToken);
            if (table == null)
            {
                result.success = false;
                result.messageForUser = "Không tìm thấy bàn này.";
                return result;
            }

            var company = db.Company.FirstOrDefault(x => x.CompanyId == table.CompanyId);
            if (company == null)
            {
                result.success = false;
                result.messageForUser = "Không tìm thấy công ty.";
                return result;
            }
            #endregion

            #region Cache chống spam
            string cacheKey = $"CallStaff_Table_{table.TableId}";
            if (_cache.Contains(cacheKey))
            {
                result.success = false;
                result.messageForUser = "\nBạn vừa gọi nhân viên. Vui lòng đợi 3 phút trước khi gọi lại.";
                return result;
            }

            // Thêm vào cache 3 phút
            _cache.Add(cacheKey, true, DateTimeOffset.Now.AddMinutes(3));
            #endregion

            #region thực thi function
            try
            {
                var firebase = new FirebaseHelper(serviceAccountPath);
                var firebaseTokens = company.UserExtension
                    .Where(x => !string.IsNullOrEmpty(x.FirebaseToken))
                    .Select(x => x.FirebaseToken)
                    .Distinct()
                    .ToList();

                foreach (var token in firebaseTokens)
                {
                    Task.Run(() => firebase.SendNotificationToTokenAsync(
                        token,
                        $"{table.TableName} gọi!",
                        "Bàn đang cần hỗ trợ.",
                        null,
                        new Dictionary<string, string>()
                    ));
                }

                result.success = true;
                result.messageForUser = "Đã gửi yêu cầu hỗ trợ thành công.";
            }
            catch (Exception ex)
            {
                result.success = false;
                result.messageForUser = "Không thể gửi thông báo.";
                result.messageForDev = ex.Message;
            }
            #endregion

            //* Kết quả hàm *
            return result;
        }
    }
}