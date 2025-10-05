using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Controllers.ExternalServices
{
    public class FirebaseDto
    {
        public class TokenRequest
        {
            public string Token { get; set; }
        }
        public class TopicMessageRequest
        {
            public string Topic { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public string ImageUrl { get; set; }
            public Dictionary<string, string> Data { get; set; }
        }
    }
}