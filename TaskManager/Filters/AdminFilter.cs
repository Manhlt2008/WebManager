using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Utils;

namespace TaskManager.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = Helper.CurrentUser;
            if (user == null || !user.IsAdmin)
            {
                var url = "/User/Login";
                filterContext.Result = new RedirectResult(url);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}