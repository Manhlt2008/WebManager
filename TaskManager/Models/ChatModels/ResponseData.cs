using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models.ChatModels
{
    public class ResponseData
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
    }
}