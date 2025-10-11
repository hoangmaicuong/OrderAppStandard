using Dapper;
using OrderApp.Areas.Admin.Controllers.AdminProduct;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminTable
{
    public class AdminTableService
    {
        private OrderAppEntities db;
        private DapperContext dapperContext = new DapperContext();
        public AdminTableService(OrderAppEntities _db)
        {
            db = _db;
        }
        public DataSet GetAll(int companyId)
        {

            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminTableModuleGetAll";
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
        public async Task<Support.ResponsesAPI> GetAllAsync(int companyId)
        {
            var result = new Support.ResponsesAPI();

            try
            {
                using (var connec = dapperContext.CreateConnection())
                {
                    await connec.OpenAsync();
                    using (var multi = await connec.QueryMultipleAsync(
                        "AdminTableModuleGetAll",
                        new { companyId },
                        commandType: CommandType.StoredProcedure))
                    {
                        var tables = (await multi.ReadAsync<dynamic>()).ToList();

                        return new Support.ResponsesAPI()
                        {
                            success = true,
                            objectResponses = new
                            {
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
        public Support.ResponsesAPI Create(int companyId, AdminTableDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Table table = new Table();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (string.IsNullOrEmpty(dto.Table.TableName))
            {
                result.success = false;
                result.messageForUser = "Tên không được bỏ trống.";
                return result;
            }
            table.CompanyId = companyId;
            table.TableName = dto.Table.TableName;
            table.IsOpen = dto.Table.IsOpen;
            table.TableToken = Guid.NewGuid();

            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Table.Add(table);
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = table.TableId
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
        public Support.ResponsesAPI Edit(AdminTableDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Table table = new Table();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            table = db.Table.FirstOrDefault(x => x.TableId == dto.Table.TableId);
            if (table == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            table.TableName = dto.Table.TableName;
            table.IsOpen = dto.Table.IsOpen;
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
        public Support.ResponsesAPI CreateNewToken(int tableId)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Table table = new Table();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            table = db.Table.FirstOrDefault(x => x.TableId == tableId);
            if (table == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            table.TableToken = Guid.NewGuid();
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
                        success = true,
                        objectResponses = new
                        {
                            tableToken = table.TableToken
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
                    string proce = "AdminTableModuleGetOrderOfTable";
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
    }
}