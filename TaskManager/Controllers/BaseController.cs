using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public User CurrentUser {
            get {
                return Helper.CurrentUser;
            }
        }
    }
}
