using Dapper;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.Controllers.ExternalServices;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace OrderApp.Areas.Admin.Controllers.AdminOrder
{
    public class AdminOrderService
    {
        private OrderAppEntities db;
        private DapperContext dapperContext = new DapperContext();
        string imagePath = ConfigurationManager.AppSettings["ProductImageUploadPath"];
        public AdminOrderService(OrderAppEntities _db)
        {
            db = _db;
        }
        public async Task<Support.ResponsesAPI> GetAllAsync(int companyId)
        {
            var result = new Support.ResponsesAPI();

            try
            {
                using (var connec = dapperContext.CreateConnection())
                {
                    await connec.OpenAsync();
                    using (var multi = await connec.QueryMultipleAsync(
                        "AdminOrderModuleGetAll",
                        new { companyId },
                        commandType: CommandType.StoredProcedure))
                    {
                        var orders = (await multi.ReadAsync<dynamic>()).ToList();
                        var tables = (await multi.ReadAsync<dynamic>()).ToList();

                        return new Support.ResponsesAPI()
                        {
                            success = true,
                            objectResponses = new
                            {
                                orders,
                                tables
                            }
                        };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Lỗi SQL
                result.success = false;
                result.messageForUser = "Lỗi cơ sở dữ liệu.";
                result.messageForDev = sqlEx.Message;
            }
            catch (Exception ex)
            {
                // Lỗi khác
                result.success = false;
                result.messageForUser = "Đã xảy ra lỗi.";
                result.messageForDev = ex.Message;
            }
            return result;
        }
        public DataSet GetAll(int companyId)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminOrderModuleGetAll";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@companyId",
                            SqlDbType = SqlDbType.Int,
                            Value = companyId
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "orders";
                            dataSet.Tables[1].TableName = "tables";
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
        public DataSet GetOrderDetails(Order order)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminOrderModuleGetOrderDetails";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@id",
                            SqlDbType = SqlDbType.Int,
                            Value = order.OrderId
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "orderDetails";
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
        public Support.ResponsesAPI RemoveOrderDetail(OrderDetail _orderDetail)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            OrderDetail orderDetail = _orderDetail;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (orderDetail == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            var order = orderDetail.Order;
            if(order == null)
            {
                result.success = false;
                result.messageForUser = "Đơn không tồn tại.";
                return result;
            }
            if (order.IsDelivered == true || order.IsFinish == true)
            {
                result.success = false;
                result.messageForUser = "Món đã giao.";
                return result;
            }
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.OrderDetail.Remove(orderDetail);
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true
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
        public Support.ResponsesAPI ChangeStatusToFinish(Order _order)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = _order;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            order.IsFinish = true;
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true
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
        public Support.ResponsesAPI OrderConfirm(Order _order, string serviceAccountPath)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = _order;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            var company = db.Company.FirstOrDefault(x => x.CompanyId == order.CompanyId);
            if(company == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            var table = db.Table.FirstOrDefault(x => x.TableId == order.TableId);
            if(table == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            order.IsConfirm = true;
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    var firebase = new FirebaseHelper(serviceAccountPath);
                    var firebaseTokens = company.UserExtension
                        .Where(x => !string.IsNullOrEmpty(x.FirebaseToken) && x.IsOrderConfirmNotification == true)
                        .Select(x => x.FirebaseToken).Distinct().ToList();
                    foreach (var token in firebaseTokens)
                    {
                        Task.Run(() => firebase.SendNotificationToTokenAsync(
                            token,
                            "Đơn xác nhận",
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
                        success = true
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
        public Support.ResponsesAPI OrderInProcess(Order _order, string serviceAccountPath)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = _order;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            var company = db.Company.FirstOrDefault(x => x.CompanyId == order.CompanyId);
            if (company == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            var table = db.Table.FirstOrDefault(x => x.TableId == order.TableId);
            if (table == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }

            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            order.IsInProcess = true;
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    var firebase = new FirebaseHelper(serviceAccountPath);
                    var firebaseTokens = company.UserExtension
                        .Where(x => !string.IsNullOrEmpty(x.FirebaseToken) && x.IsOrderInProcessNotification == true)
                        .Select(x => x.FirebaseToken).Distinct().ToList();
                    foreach (var token in firebaseTokens)
                    {
                        Task.Run(() => firebase.SendNotificationToTokenAsync(
                            token,
                            "Đơn chuẩn bị",
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
                        success = true
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
        public Support.ResponsesAPI OrderDelivered(Order _order)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = _order;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            order.IsDelivered = true;
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true
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
        public Support.ResponsesAPI RestoreOrder(Order _order)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = _order;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            order.IsFinish = false;
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true
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
        public Support.ResponsesAPI DeliveredOrderDetail(OrderDetail _orderDetail, string serviceAccountPath)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            OrderDetail orderDetail = _orderDetail;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            var company = db.Company.FirstOrDefault(x => x.CompanyId == orderDetail.Order.CompanyId);
            if (company == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            var table = db.Table.FirstOrDefault(x => x.TableId == orderDetail.Order.TableId);
            if (table == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }

            if (orderDetail == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            var order = orderDetail.Order;
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Đơn không tồn tại.";
                return result;
            }
            var product = orderDetail.Product;
            if (product == null)
            {
                result.success = false;
                result.messageForUser = "Sản phẩm không tồn tại.";
                return result;
            }
            #endregion
            if (orderDetail.IsDelivered == null)
            {
                orderDetail.IsDelivered = false;
            }
            orderDetail.IsDelivered = !orderDetail.IsDelivered;
            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SaveChanges();
                    transaction.Commit();

                    var firebase = new FirebaseHelper(serviceAccountPath);
                    var firebaseTokens = company.UserExtension
                        .Where(x => !string.IsNullOrEmpty(x.FirebaseToken) && x.IsOrderInProcessNotification == true)
                        .Select(x => x.FirebaseToken).Distinct().ToList();
                    foreach (var token in firebaseTokens)
                    {
                        Task.Run(() => firebase.SendNotificationToTokenAsync(
                            token,
                            $"{product.ProductName}",
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
                        success = true
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
    }
}