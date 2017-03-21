using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models.ChatModels
{
    public class ClientChatUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { set; get; }
        public string OnlineStatus { set; get; }
    }
}