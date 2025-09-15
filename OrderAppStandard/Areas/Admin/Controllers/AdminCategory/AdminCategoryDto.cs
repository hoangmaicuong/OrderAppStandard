using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminCategory
{
    public class AdminCategoryDto
    {
        public class UpdateDto
        {
            public UpdateDto()
            {
                Category = new CategoryDto();
            }
            public CategoryDto Category { get; set; }
        }
        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int No { get; set; }
        }
    }
}