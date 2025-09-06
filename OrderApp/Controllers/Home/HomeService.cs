using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using OrderApp.Areas.Admin.Controllers.AdminProduct;

namespace OrderApp.Controllers.Home
{
    public class HomeService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        public DataSet GetAll()
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
            return null;
            //var result = new Support.ResponsesAPI();
            //#region khởi tạo tham số
            //Models.Order product = new Product();

            //#endregion

            //#region Kiểm tra điều kiện thực thi function
            //// Check.. (điều kiện để thực thi)
            //if (string.IsNullOrEmpty(dto.Product.ProductName))
            //{
            //    result.success = false;
            //    result.messageForUser = "Tên không được bỏ trống.";
            //    return result;
            //}
            //product.ProductName = dto.Product.ProductName;
            //product.ProductPrice = dto.Product.ProductPrice;
            //product.ProductDescription = dto.Product.ProductDescription;
            //product.CategoryId = dto.Product.CategoryId;
            //product.IsActive = dto.Product.IsActive;

            //#endregion

            //#region thực thi function
            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        db.Product.Add(product);
            //        db.SaveChanges();
            //        transaction.Commit();

            //        result = new Support.ResponsesAPI
            //        {
            //            success = true,
            //            objectResponses = new
            //            {
            //                id = product.ProductId
            //            }
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();

            //        result = new Support.ResponsesAPI
            //        {
            //            success = false,
            //            messageForUser = Support.ResponsesAPI.MessageAPI.messageException,
            //            messageForDev = ex.Message
            //        };
            //    }
            //}
            //#endregion

            ////* Kết quả hàm *
            //return result;
        }
    }
}