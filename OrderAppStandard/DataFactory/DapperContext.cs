using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OrderApp.DataFactory
{
    public class DapperContext
    {
        public DapperContext() { }
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        //Tất cả dùng chung nên tạo 1 lần
        public static readonly DapperContext dapperContext = new DapperContext();
    }
}