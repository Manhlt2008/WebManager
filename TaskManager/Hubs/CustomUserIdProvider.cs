using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TaskManager.Utils;

namespace TaskManager.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            var user = Helper.CurrentUser;
            return user != null ? user.Id.ToString() : string.Empty;
        }
    }
}