using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminReport
{
    public class AdminReportService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        string imagePath = ConfigurationManager.AppSettings["ProductImageUploadPath"];
        public DataSet GetFilter(int companyId, DateTime startDate, DateTime endDate, string searchKey = null)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminReportModuleGetFilter";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@companyId",
                            SqlDbType = SqlDbType.Int,
                            Value = companyId
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@startDate",
                            SqlDbType = SqlDbType.DateTime,
                            Value = startDate
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@endDate",
                            SqlDbType = SqlDbType.DateTime,
                            Value = endDate
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@searchKey",
                            SqlDbType = SqlDbType.NVarChar,
                            Value = searchKey
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "orders";
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