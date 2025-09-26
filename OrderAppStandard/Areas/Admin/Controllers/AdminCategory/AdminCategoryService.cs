using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using OrderApp.Areas.Admin.Controllers.AdminTable;

namespace OrderApp.Areas.Admin.Controllers.AdminCategory
{
    public class AdminCategoryService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        string imagePath = ConfigurationManager.AppSettings["ProductImageUploadPath"];
        public DataSet GetAll(int companyId)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminCategoryModuleGetAll";
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
                            dataSet.Tables[0].TableName = "categorys";
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
        public Support.ResponsesAPI Create(AdminCategoryDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Category category = new Category();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            if (string.IsNullOrEmpty(dto.Category.CategoryName))
            {
                result.success = false;
                result.messageForUser = "Tên không được bỏ trống.";
                return result;
            }
            category.CategoryName = dto.Category.CategoryName;
            category.No = dto.Category?.No;

            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Category.Add(category);
                    db.SaveChanges();
                    transaction.Commit();

                    result = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = category.CategoryId
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
        public Support.ResponsesAPI Edit(AdminCategoryDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Category category = new Category();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            category = db.Category.FirstOrDefault(x => x.CategoryId == dto.Category.CategoryId);
            if (category == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            category.CategoryName = dto.Category.CategoryName;
            category.No = dto.Category?.No;
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
        public Support.ResponsesAPI Delete(int categoryId)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Category category = new Category();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            category = db.Category.FirstOrDefault(x => x.CategoryId == categoryId);
            if (category == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            
            #endregion

            db.Category.Remove(category);
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