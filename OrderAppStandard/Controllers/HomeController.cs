using Dapper;
using OrderApp.Controllers.Home;
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

                    string sql = "SELECT TOP 1 * FROM Company WHERE Slug = @Slug";

                    var company = await conn.QueryFirstOrDefaultAsync<HomeDto.CompanyDto>(sql, new { Slug = CompanySlug });

                    if (company == null)
                    {
                        return RedirectToAction("NotFound", "Home");
                    }

                    ViewBag.TableId = tableId;
                    ViewBag.TableToken = tableToken;
                    ViewBag.CompanySlug = CompanySlug;

                    ViewBag.Summary = company.Summary;
                    ViewBag.Address = company.Address;
                    ViewBag.Phone1 = company.Phone1;
                    ViewBag.Phone2 = company.Phone2;
                    ViewBag.Email1 = company.Email1;
                    ViewBag.Email2 = company.Email2;

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