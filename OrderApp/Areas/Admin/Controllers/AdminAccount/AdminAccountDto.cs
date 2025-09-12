using OrderApp.Areas.Admin.Controllers.AdminCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    public class AdminAccountDto
    {
        public class UpdateDto
        {
            public UpdateDto()
            {
                AspNetUser = new AspNetUser();
            }
            public AspNetUser AspNetUser { get; set; }
        }
        public class AspNetUser
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }
    }
}