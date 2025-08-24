using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminProduct
{
    public class AdminProductService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        string imagePath = ConfigurationManager.AppSettings["ProductImageUploadPath"];
        public DataSet GetAll()
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminProductModuleGetAll";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "products";
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
        public Support.ResponsesAPI Update(AdminProductDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Product product;

            #endregion

            #region Kiểm tra điều kiện thực thi function
            
            // Check..
            product = db.Product.FirstOrDefault(x => x.ProductId == dto.Product.ProductId);
            if (product == null)
            {
                result.success = false;
                result.messageForUser = "Sửa thất bại! sản phẩm này không tồn tại.";
                return result;
            }

            // Mapper..
            product.ProductPrice = dto.Product.ProductPrice;
            product.ProductName = dto.Product.ProductName;
            product.ProductDescription = dto.Product.ProductDescription;

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
    }
}