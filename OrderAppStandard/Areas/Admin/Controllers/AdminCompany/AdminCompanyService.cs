using Dapper;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminCompany
{
    public class AdminCompanyService
    {
        private OrderAppEntities db;
        private DapperContext dapperContext;
        public AdminCompanyService(OrderAppEntities _db, DapperContext _dapperContext)
        {
            db = _db;
            dapperContext = _dapperContext;
        }
        public DataSet GetDetail(int companyId)
        {
            using (var connec = dapperContext.CreateConnection())
            {
                connec.Open();
                try
                {
                    string proce = "AdminCompanyModuleGetDetail";
                    using (var cmd = new SqlCommand(proce, connec))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@id",
                            SqlDbType = SqlDbType.Int,
                            Value = companyId
                        });
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = "companys";
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
        public Support.ResponsesAPI Edit(int companyId, string userId, AdminCompanyDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            #region khởi tạo tham số
            Company company = new Company();

            #endregion

            #region Kiểm tra điều kiện thực thi function
            // Check.. (điều kiện để thực thi)
            company = db.Company.FirstOrDefault(x => x.CompanyId == companyId);
            if (company == null)
            {
                result.success = false;
                result.messageForUser = "Data này không tồn tại.";
                return result;
            }
            if(company.CompanyOwnerId != userId)
            {
                result.success = false;
                result.messageForUser = "Chủ sở hữu mới có quyền chỉnh sửa.";
                return result;
            }
            if (string.IsNullOrEmpty(dto.Company.Slug))
            {
                result.success = false;
                result.messageForUser = "Tên miền công ty không bỏ trống.";
                return result;
            }
            if (dto.Company.Slug.Length > 100)
            {
                result.success = false;
                result.messageForUser = "Tên miền không vượt 100 ký tự.";
                return result;
            }
            if(!Regex.IsMatch(dto.Company.Slug, "^[a-z0-9-]+$"))
            {
                result.success = false;
                result.messageForUser = "Tên miền chỉ được chứa chữ thường, số và dấu gạch ngang (-), không được có khoảng trắng hoặc ký tự đặc biệt.";
                return result;
            }
            bool slugExists = db.Company.Any(c => c.Slug == dto.Company.Slug && c.CompanyId != companyId);
            if (slugExists)
            {
                result.success = false;
                result.messageForUser = "Tên miền đã tồn tại, vui lòng chọn tên khác.";
                return result;
            }

            company.CompanyName = dto.Company.CompanyName;
            company.Slug = dto.Company.Slug;
            company.Address = dto.Company.Address;
            company.Phone1 = dto.Company.Phone1;
            company.Phone2 = dto.Company.Phone2;
            company.Email1 = dto.Company.Email1;
            company.Email2 = dto.Company.Email2;
            company.Summary = dto.Company.Summary;

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