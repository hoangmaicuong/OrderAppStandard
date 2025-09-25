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

namespace OrderApp.Controllers.Home
{
    public class HomeService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
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
        public Support.ResponsesAPI CreateOrder(HomeDto.CreateOrderDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Order order = new Order();

            #endregion
            bool existTable = db.Table.Any(x => x.TableId == dto.Order.TableId && x.TableToken == dto.Order.TableToken);
            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (dto.Order.TableId < 1 || !existTable)
            {
                result.success = false;
                result.messageForUser = "Chưa có bàn.";
                return result;
            }
            order.TableId = dto.Order.TableId;
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
    }
}