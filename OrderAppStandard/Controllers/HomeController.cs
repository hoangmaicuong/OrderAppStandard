using Dapper;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DapperContext dapperContext = DapperContext.dapperContext;
        public ActionResult Index()
        {
            return View();
        }
        [Route("{CompanySlug}")]
        public async Task<ActionResult> Shop(string CompanySlug, int tableId = 0, Guid? tableToken = null)
        {
            if (string.IsNullOrEmpty(CompanySlug))
            {
                return RedirectToAction("NotFound", "Home");
            }
            try
            {
                using (var conn = dapperContext.CreateConnection())
                {
                    await conn.OpenAsync();

                    string sql = "SELECT TOP 1 CompanyId FROM Company WHERE Slug = @Slug";

                    int companyId = await conn.QueryFirstOrDefaultAsync<int>(sql, new { Slug = CompanySlug });

                    if (companyId == 0)
                    {
                        return RedirectToAction("NotFound", "Home");
                    }

                    ViewBag.TableId = tableId;
                    ViewBag.TableToken = tableToken;
                    ViewBag.CompanySlug = CompanySlug;

                    return View();
                }
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                // Log.Error(ex);
                return new HttpStatusCodeResult(500, "Internal Server Error");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Route("not-found")]
        public ActionResult NotFound()
        {
            return View();
        }
    }
}