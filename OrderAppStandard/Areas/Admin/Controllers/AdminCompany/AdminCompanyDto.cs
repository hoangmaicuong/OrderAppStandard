using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminCompany
{
    public class AdminCompanyDto
    {
        public class UpdateDto
        {
            public UpdateDto()
            {
                Company = new CompanyDto();
            }
            public CompanyDto Company { get; set; }
        }
        public class CompanyDto
        {
            public string CompanyName { get; set; }
            public string CompanyOwnerId { get; set; }
            public string Slug { get; set; }
            public string Address { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Summary { get; set; }
        }
    }
}