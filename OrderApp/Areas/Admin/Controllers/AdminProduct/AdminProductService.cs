using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using OrderApp.Areas.Admin.Controllers.AdminSample;
using System.IO;
using static System.Net.WebRequestMethods;

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
        public Support.ResponsesAPI Create(AdminProductDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Product product = new Product();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (string.IsNullOrEmpty(dto.Product.ProductName))
            {
                result.success = false;
                result.messageForUser = "Tên không được bỏ trống.";
                return result;
            }
            product.ProductName = dto.Product.ProductName;
            product.ProductPrice = dto.Product.ProductPrice;
            product.ProductDescription = dto.Product.ProductDescription;
            product.CategoryId = dto.Product.CategoryId;
            product.IsActive = dto.Product.IsActive;

            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Product.Add(product);
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = product.ProductId
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
        public Support.ResponsesAPI Edit(AdminProductDto.UpdateDto dto)
        {
            // Chỉ cho logic chạy vào các trường hợp đã biết trước!
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Product product = new Product();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            product = db.Product.FirstOrDefault(x => x.ProductId == dto.Product.ProductId);
            if (product == null)
            {
                result.success = false;
                result.messageForUser = "Sản phẩm này không tồn tại.";
                return result;
            }

            product.ProductName = dto.Product.ProductName;
            product.ProductPrice = dto.Product.ProductPrice;
            product.ProductDescription = dto.Product.ProductDescription;
            product.CategoryId = dto.Product.CategoryId;
            product.IsActive = dto.Product.IsActive;
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
        public Support.ResponsesAPI UploadImage(int productId, HttpRequest Request, HttpServerUtility Server)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            var file = Request.Files[0];
            Product product = db.Product.FirstOrDefault(x => x.ProductId == productId);

            #endregion
            if (product == null)
            {
                return new Support.ResponsesAPI
                {
                    success = false,
                    messageForUser = "Không tìm thấy sản phẩm"
                };
            }

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(imagePath), fileName);

                // Tùy chọn: Đổi tên file nếu trùng
                var FileId = Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);

                var uniquePath = Path.Combine(Server.MapPath(imagePath), FileId);

                // 🔹 kiểm tra có ảnh cũ không
                if (product.ProductImage != null && !string.IsNullOrEmpty(product.ProductImage.FileId))
                {
                    var oldFilePath = Path.Combine(Server.MapPath(imagePath), product.ProductImage.FileId);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath); // xóa ảnh cũ
                    }
                }

                // lưu ảnh mới
                file.SaveAs(uniquePath);

                // Lưu vào DB
                product.ProductImage = new ProductImage
                {
                    FileId = FileId
                };
                db.SaveChanges();
                result = new Support.ResponsesAPI
                {
                    success = true,
                    objectResponses = new
                    {
                        fileId = FileId
                    }
                };
            }
            return result;
        }
    }
}