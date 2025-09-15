using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderApp
{
    public static class Support
    {
        public class ResponsesAPI
        {
            public bool success { get; set; } = false;
            public string messageForUser { get; set; } = "";
            public string messageForDev { get; set; } = "";
            public object objectResponses { get; set; } = null;
            public static class MessageAPI
            {
                public const string failTitle = "Thao tác thất bại";
                public const string messageException = "Thao tác thất bại!!!";
            }
        }
    }
}