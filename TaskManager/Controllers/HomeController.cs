using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Entity;

namespace TaskManager.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var alert = AlertBO.GetAll(CurrentUser.Id) ?? new List<Alert>();
            return View(alert);
        }

        public ActionResult Detail(int referenceId, int typeId, int id)
        {
            AlertBO.Delete(id);
            switch (typeId)
            {
                case (int) AlertType.AssignTask:
                    return RedirectToAction("MyTask", "AssignTask");
                case (int)AlertType.Task:
                    return RedirectToAction("Index", "Task");
                case (int)AlertType.Comment:
                    return RedirectToAction("Index", "Report");
                case (int)AlertType.Event:
                    return RedirectToAction("Index", "Appointment");
                case (int)AlertType.Report:
                    return RedirectToAction("ListReport", "Report");
            }

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            AlertBO.Delete(id);
            return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}
