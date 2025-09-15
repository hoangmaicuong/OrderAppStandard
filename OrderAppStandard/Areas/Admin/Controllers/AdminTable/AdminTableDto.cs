using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminTable
{
    public class AdminTableDto
    {
        public class UpdateDto
        {
            public UpdateDto()
            {
                Table = new TableDto();
            }
            public TableDto Table { get; set; }
        }
        public class TableDto
        {
            public int TableId { get; set; }
            public string TableName { get; set; }
            public Nullable<bool> IsOpen { get; set; }
        }
    }
}