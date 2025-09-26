using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using OrderApp.Areas.Admin.Controllers.AdminCategory;
using OrderApp.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    public class AdminAccountService
    {
        private OrderAppEntities db = new OrderAppEntities();
        private DapperContext dapperContext = new DapperContext();
        string imagePath = ConfigurationManager.AppSettings["ProductImageUploadPath"];
        private ApplicationUserManager UserManager;
        public AdminAccountService()
        {
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }
        public DataSet GetAll(int companyId)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminAccountModuleGetAll";
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
                            dataSet.Tables[0].TableName = "aspNetUsers";
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
        public async Task<Support.ResponsesAPI> Create(int companyId, AdminAccountDto.UpdateDto dto)
        {
            var response = new Support.ResponsesAPI();
            #region Khởi tạo tham số
            var user = new ApplicationUser
            {
                UserName = dto.AspNetUser.UserName,
                PhoneNumber = dto.AspNetUser.PhoneNumber
            };
            UserExtension userExtension;
            #endregion

            #region Kiểm tra điều kiện
            if (string.IsNullOrEmpty(dto.AspNetUser.Password))
            {
                response.success = false;
                response.messageForUser = "Mật khẩu không được bỏ trống.";
                return response;
            }
            #endregion

            #region thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var identityResult = await UserManager.CreateAsync(user, dto.AspNetUser.Password);

                    if (!identityResult.Succeeded)
                    {
                        transaction.Rollback();
                        response.success = false;
                        response.messageForUser = string.Join(", ", identityResult.Errors);
                        return response;
                    }
                    userExtension = new UserExtension
                    {
                        AspNetUserId = user.Id,
                        CompanyId = companyId,
                    };
                    db.UserExtension.Add(userExtension);
                    db.SaveChanges();

                    transaction.Commit();

                    response = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = user.Id
                        }
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response = new Support.ResponsesAPI
                    {
                        success = false,
                        messageForUser = Support.ResponsesAPI.MessageAPI.messageException,
                        messageForDev = ex.ToString()
                    };
                }
            }
            #endregion

            //* Kết quả hàm *
            return response;
        }
        public async Task<Support.ResponsesAPI> Edit(AdminAccountDto.UpdateDto dto)
        {
            var response = new Support.ResponsesAPI();

            #region Kiểm tra điều kiện
            if (dto.AspNetUser == null || string.IsNullOrEmpty(dto.AspNetUser.Id))
            {
                response.success = false;
                response.messageForUser = "Người dùng không hợp lệ.";
                return response;
            }

            var user = await UserManager.FindByIdAsync(dto.AspNetUser.Id);
            if (user == null)
            {
                response.success = false;
                response.messageForUser = "Không tìm thấy người dùng.";
                return response;
            }
            #endregion

            // Cập nhật các trường cho user
            user.PhoneNumber = dto.AspNetUser.PhoneNumber;
            user.UserName = dto.AspNetUser.UserName;

            #region Thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Update user
                    var identityResult = await UserManager.UpdateAsync(user);
                    if (!identityResult.Succeeded)
                    {
                        transaction.Rollback();
                        response.success = false;
                        response.messageForUser = string.Join(", ", identityResult.Errors);
                        return response;
                    }

                    transaction.Commit();

                    response = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = user.Id
                        }
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response = new Support.ResponsesAPI
                    {
                        success = false,
                        messageForUser = Support.ResponsesAPI.MessageAPI.messageException,
                        messageForDev = ex.ToString()
                    };
                }
            }
            #endregion

            return response;
        }
        public async Task<Support.ResponsesAPI> ChangePassword(AdminAccountDto.UpdateDto dto)
        {
            var response = new Support.ResponsesAPI();

            #region Kiểm tra điều kiện
            if (string.IsNullOrEmpty(dto.AspNetUser.Id) || string.IsNullOrEmpty(dto.AspNetUser.NewPassword))
            {
                response.success = false;
                response.messageForUser = "Dữ liệu không hợp lệ.";
                return response;
            }

            var user = await UserManager.FindByIdAsync(dto.AspNetUser.Id);
            if (user == null)
            {
                response.success = false;
                response.messageForUser = "Không tìm thấy người dùng.";
                return response;
            }
            #endregion

            #region Thực thi function
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Xóa mật khẩu cũ
                    var removeResult = await UserManager.RemovePasswordAsync(user.Id);
                    if (!removeResult.Succeeded)
                    {
                        transaction.Rollback();
                        response.success = false;
                        response.messageForUser = string.Join(", ", removeResult.Errors);
                        return response;
                    }

                    // Thêm mật khẩu mới
                    var addResult = await UserManager.AddPasswordAsync(user.Id, dto.AspNetUser.NewPassword);
                    if (!addResult.Succeeded)
                    {
                        transaction.Rollback();
                        response.success = false;
                        response.messageForUser = string.Join(", ", addResult.Errors);
                        return response;
                    }

                    transaction.Commit();

                    response = new Support.ResponsesAPI
                    {
                        success = true,
                        objectResponses = new
                        {
                            id = user.Id
                        }
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    response = new Support.ResponsesAPI
                    {
                        success = false,
                        messageForUser = Support.ResponsesAPI.MessageAPI.messageException,
                        messageForDev = ex.ToString()
                    };
                }
            }
            #endregion

            return response;
        }
    }
}